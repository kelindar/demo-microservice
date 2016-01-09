var app = new Vue({
    el: '#wrapper',
    data: {
        name: '',
        avatar: '',
        message: '',
        users: [],
        messages: []
    },
    methods: {
        sendMessage: function () {
            var text = this.message.trim()
            if (text) {
                this.message = '';
                emitter.publish({
                    key: '9sj2RAjC6a0BcuEa1BUkjjVePfNXWnB6',
                    channel: 'messages/v1/',
                    message: JSON.stringify({
                        name: this.name,
                        avatar: this.avatar,
                        text: text
                    })
                });
            }
        }
    }
})

emitter = new Emitter("192.168.56.102", 8080);
emitter.on('connect', function(){
    
    console.log(getPersistentVisitorId());
    emitter.subscribe({
        key: 'eFg7dFDJEd-7NGN3GeGPX3gBgGTWjhxX',
        channel: 'sentiment-response/v1/'
    });
    
    emitter.subscribe({
        key: '_cLHiUr0dvGj7RYP-3bcV9DnxMJzhMGJ',
        channel: 'user-notify/v1/'
    });
    
    emitter.subscribe({
        key: '9sj2RAjC6a0BcuEa1BUkjjVePfNXWnB6',
        channel: 'messages/v1/'
    });
    
    ping(true);
});

/**
 * Processes incoming messages.
 */
emitter.on('message', function(msg){
    if (msg.channel.startsWith("user-notify")) {
        onUserNotify(msg.asObject());
    } 
    else if (msg.channel.startsWith("messages")) {
        onMessage(msg.asObject());
    }
    else if (msg.channel.startsWith("sentiment-response")) {
        onSentimentAnalyzed(msg.asObject());
    }
});


function onUserNotify(msg){
    // append the data URI prefix 
    msg.avatar = "data:image/png;base64," + msg.avatar;
    
    // is it us?
    if (msg.token == getPersistentVisitorId()){
        app.$data.name = msg.name;
        app.$data.avatar = msg.avatar;
        
        // every second, ping
        setInterval(function(){
            ping(false);
        }, 1000);
    } 
    
    if (msg.active) {
        // add the user to the list
        app.$data.users.push(msg);
    }else{
        // remove the user from the list
        for (var i=app.$data.users.length-1; i>=0; i--) {
            if (app.$data.users[i].token == msg.token) 
                app.$data.users.splice(i,1);
        }
    }
    
}

/**
 * Occurs when a chat message is received
 */
function onMessage(msg){
    // add an id for the message
    msg.id = getRequestId();
    msg.sentiment = '';
    msg.class = 'label label-default';
    
    // push the messages to the message list
    app.$data.messages.push(msg);
    
    // analyze the sentiment
    analyzeSentiment(msg.id, msg.text)
}

/**
 * Occurs when a sentiment analysis response is received
 */
function onSentimentAnalyzed(msg){
    console.log(msg);
    for (var i=app.$data.messages.length-1; i>=0; i--) {
        if (app.$data.messages[i].id === msg.id) {
            app.$data.messages[i].sentiment = msg.sentiment;
            if(msg.sentiment > 0)
                app.$data.messages[i].class = 'label label-primary';
            if(msg.sentiment < 0)
                app.$data.messages[i].class = 'label label-danger';
        }
    }
}


/**
 * Tells the remote server that we're active.
 */
function ping(greet){
    emitter.publish({
        key: 'LoD3PKxDVsNrruAGdnwbZVH4E8ZPqrJN',
        channel: 'user-ping/v1/',
        message: JSON.stringify({
            token: getPersistentVisitorId(),
            greet: greet
        })
    });
}

/**
 * Requests the sentiment analysis for some text.
 */
function analyzeSentiment(id, text){
    // publish the request
    emitter.publish({
        key: 'KDucU3hgihsZx5ofW54fU3r5E_1EWIKg',
        channel: 'sentiment-request/v1/',
        message: JSON.stringify( {
            id: id,
            reply: "me",
            text: text
        })
    });
}


// connect to the message broker
emitter.connect();