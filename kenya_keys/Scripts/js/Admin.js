
var admin = {
    photo: {
        openGallery: function () {
            $.ajax({
                type: 'GET',
                url : '/photo/list',
                success: function (html) {
                    var div = 	"<div id='photo-gallery-popup' style='position:absolute;top:0;left:0;right:0;bottom:0; background-color:Black;'>" +
                               		"<div><a href=''></a></div>"+
                               		"<div style='position:absolute;top :200px; left :33%; background-color:white; height : 225px; width:350px; padding:25px;'>" + html + "</div>" +
                            	"</div>";
                    $("body").append(div);
                }
            });
        }
    },
    content: {
        current : {
            controller : null,
            action : null,
            elementId : null
        },
        edit: function (controller, action, elementId) {
            this.current.contentController = controller;
            this.current.contentAction = action;
            this.current.elementId = elementId;

            $.ajax({
                url: '/Admin/Edit',
                type: 'GET',
                data: { contentController: controller, contentAction: action, elementId : elementId },
                success: function (html) {
                    document.getElementById(elementId).innerHTML = html;
                    setTimeout(function () {
                        CKEDITOR.replace("element_html", {
                            toolbar: 'Basic',
                            uiColor: '#9AB8F3'
                        });
                    }, 250);
                }
            });
        },

        save: function (formId) {
            var current = this.current;
            $("input[name=elementHtml]").val(escape(CKEDITOR.instances.element_html.getData()));
            var data = $(document.forms[formId]).serialize();

            $.ajax({
                url: '/Admin/Save',
                type: 'POST',
                data: data,
                success: function (html) {
                    $.ajax({
                        url: '/Admin/View',
                        data: current,
                        success: function (html) {
                            document.getElementById(current.elementId).innerHTML = html;
                        }
                    });
                    
                }
            });
        }
    }

};