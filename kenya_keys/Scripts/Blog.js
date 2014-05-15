
var Blog = {
    // properties
    loadEditor: function () {

    	
        CKEDITOR.replace("blog_html_ta", {
            toolbar: 'Basic',
            uiColor: '#9AB8F3'
        });
    },

    add: function (blogId) {
        var data = blogId ? { blogId : blogId }  : {};
        $.ajax({
        	type : 'GET',
            url: "/Blog/Edit",
            data: data,
            success: function (html) {
                createPopup({ html : html, onload : Blog.loadEditor });
            }
        });
    },

    close : function() {
    	$("#kk-popup").remove();
    },

    deleteBlog: function (blogId) {
        if (confirm("Are you sure you want to delete this project? NOTE : If you want to hide the post you don't have to delete it but rather select hide when editing the blog.")) {
            $.ajax({
                url: '/Blog/Delete',
                type: 'GET',
                data: { blogId : blogId },
                success: function (response) {
                    window.location = "/Home/Blog";
                },
				error : function(result) {
					console.log(result);
					window.location = "/Home/Blog";
                }
            });
        }
    },

    // methods
    submit: function () {
        // set the value for the html
        $("input[name=blog_html]").val(escape(CKEDITOR.instances.blog_html_ta.getData()));
        var data = $(document.forms["blog-edit"]).serialize();
        $.ajax({
            url: '/Blog/Save',
            type: 'POST',
            data: data,
            // dataType: 'application/json',
            success: function (result) {
                window.location = "/Blog/View?blogId="+result.BlogID.toString();
            },
            error: function (result) {
                if (result.responseText) {
                	try {
                		var blog = JSON.parse(result.responseText);
                		window.location = "/Blog/View?blogId="+blog.BlogID.toString();
                	} catch (ex) {
                		console.log('Unable to parse');
                	}
                }
            }
        });

    }
};
