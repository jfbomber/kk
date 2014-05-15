var KenyaKeys = {
    login: function () {
        var username = document.getElementById('username').value;
        var password = document.getElementById('password').value;

        if (username.length === 0 || password.length === 0) {
            this.invalidLogin('Username and Password are required');
            return;
        }

        $.ajax({
            url: '/home/login',
            type : 'POST',
            data: {
                username: username,
                password: password
            },
            success: function (response) {
                window.location = window.location;
            }
        });
    },

    logout : function() {
        $.ajax({
            url: '/Home/Logout',
            type: 'GET',
            data: { },
            success: function (response) {
                window.location = window.location;
            }
        });
    },

    invalidLogin: function (message) {
        var messageLabel = document.getElementById('messageLabel');
        message = message ? message : "Invalid login";

        var label = $(messageLabel);
        if (label.hasClass('success')) {
            label.removeClass('success');
        }

        label.addClass('warning');
        messageLabel.innerHTML = message;
    }
    
    
};