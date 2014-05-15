/**
 * @prop <Integer> project_id
 * @prop <String> project_name
 * @prop <String> project_description
 * @prop <Image> project_image
 * @prop <Boolean> project_hide
 *
 * @function add()
 */



Project = {
    // properties
    project_id: null,
    project_name: null,
    project_description: null,
    project_image: null,
    project_hide: null,



    defaultContent: null,
    _tempPhoto: null,

    loadEditor: function () {
        CKEDITOR.replace("ProjectHtml", {
            toolbar: 'Basic',
            uiColor: '#9AB8F3'
        });
    },

    edit: function (projectId, parentId) {
        projectId = projectId || null;
        parentId = parentId || null;
        $.ajax({
            url: "/Project/Edit",
            data: { project_id: projectId, parent_id : parentId },
            success: function (html) {
            	var popup = document.getElementById("project-popup");
                if (!popup) {
                	popup = document.createElement("div");
                	popup.setAttribute("id", "project-popup");
                	popup.setAttribute("style", "padding : 15px;  position : absolute; background-color : white; z-index : 200; left : 25px; top : 25px; border: solid 1px black; bottom : 25px; right : 25px; overflow:scroll;");
                }
                popup.innerHTML = unescape(html);

                $("body").append(popup);

                setTimeout(Project.loadEditor, 1000);

            },
			error : function(err) {
				console.log(err);
			}
        });
    },

	closeWindow : function() {
		$("#project-popup").remove();
    },



    /**
     * Deletes a project
     */
    deleteProject: function (projectId) {
        if (confirm("Are you sure you want to delete this project?")) {
            $.ajax({
                url: '/Project/Delete',
                type: 'GET',
                data: { projectId: projectId },
                success: function (response) {
                    window.location = window.location;
                },
				error : function(err) {
					window.location = window.location + "?error="+err;
                }
            });
        }
    },


    uploadPhoto: function (s) {
        var form = document.forms.AddPhoto;
        form.submit();

        var photoUploader = document.getElementById("photo-uploader");

        setTimeout(function () {
            $.ajax({
                url: '/Photo/Last',
                data: {},
                type: 'GET',
                success: function (result) {
                    if (result != null) {
                        result = JSON.parse(result);
                        Project._tempPhoto = result;
                        document.getElementById("uploaded-photo-id").value = result.photo_id;
                        $(photoUploader)
                            .find($("div#photo-uploader input[type=file]"))
                            .replaceWith("<img src='/Photo/GetPhoto?photoId=" + result.photo_id.toString() + "' alt='" + result.photo_id.toString() + "' />");


                    }
                }
            });
        }, 500);


    },

    // methods
    submit: function () {
        // set the value for the html
        $("input[name=proj_html]").val(escape(CKEDITOR.instances.ProjectHtml.getData()));
        $('[name=ProjectHtml]').val("");
        var data = $(document.forms.AddProject).serialize();
        $.ajax({
            url: '/Project/Save',
            type: 'POST',
            data: data,
            dataType: 'application/json',
            success: function (result) {
                window.location = window.location;

            },
            error: function (result) {
                $("#project-popup").remove();
                console.log(result);
                window.location = window.location;
            }


        });

    },


    showProject: function (sender, projectId) {
    	
        // clear old border 
        if (!sender) {
        	sender = document.getElementById("project-tile-"+projectId);
        }



        $("div.projects div.project").css({ backgroundColor : '#e2d4aa' });
       	$(sender).css({ backgroundColor : 'Green' });

        $.ajax({
            url: '/Project/Show?projectId=' + projectId.toString(),
            type: 'GET',
            success: function (projectHtml) {
                var projectContent = document.getElementById("project-content");
                if (Project.defaultContent === null) {
                    Project.defaultContent = projectContent.innerHTML;
                }
                projectContent.innerHTML = unescape(projectHtml);
                Project.getProjectMenu(projectId);
            }, 
			error : function(err) {
				console.log("Error", err.responseText);
            }
        });
    },



    /**
     *
     *
     */
    getProjectMenu : function(projectId, parentId) {
    	projectId = projectId || null;
    	parentId = parentId || null;
    	$.ajax({
            url: '/Project/Index',
            data : {
            	project_id : projectId,
            	parent_id : parentId,
            },
            type: 'GET',
            success: function (menuHTML) {
                var projectmenu = document.getElementById("project-pages");
                if (projectmenu) {
                	projectmenu.innerHTML = menuHTML;
                }
            }, 
			error : function(err) {
				console.log("Error", err.responseText);
            }
        });

    },



    showMenu: function (e, projectId) {
        e = e || window.event;
        // check to see if the menu is already showing
        if ($("div.menu").length > 0) { return; }
        // 
        var div = document.createElement("div");
        div.setAttribute('id', 'contextMenu');
        if (div.addEventListener) {
            div.addEventListener('click', function (e) {
                alert('click');
                $(div).remove();
            });
        } else {
            div.attachEvent('oncontextmenu', function () {
                alert('click');
                $(div).remove();
            });
        }

        // set the menu attributes
        var pos = getContextMenuPosition(e, div);
        div.setAttribute("class", "menu");
        div.setAttribute("style", "top:" + pos.y + "px;left:" + pos.x + "px;");
        div.innerHTML = "Menu Item";
        $(e.target).append(div);
        if (window.event) {
            window.event.returnValue = false;
        } else {
            e.preventDefault();
        }
    },

    hideMenu : function(e) {
        if (!$(e.target).hasClass("")) {
            $("li.admin-context-menu").remove();
        }
    },


    showPage : function(projectId, parentId) {
    	 $.ajax({
            url: '/Project/Show?projectId=' + projectId.toString(),
            type: 'GET',
            success: function (projectHtml) {
                var projectContent = document.getElementById("project-content");
                if (Project.defaultContent === null) {
                    Project.defaultContent = projectContent.innerHTML;
                }
                projectContent.innerHTML = unescape(projectHtml);
                Project.getProjectMenu(projectId, parentId);
            }, 
			error : function(err) {
				console.log("Error", err.responseText);
            }
        });	
    },

    savePage : function() {
     	$("input[name=page_html]").val(escape(CKEDITOR.instances.PageHtml.getData()));
     	var data = $(document.forms.ProjectPage).serialize();
     	var projectId = $("input[name=project_id]").val();

     	var callback = function(response) {
     		$("#project-popup").remove();
     		Project.showProject(null, projectId);
     	};

        $.ajax({
            url: '/Project/SavePage',
            type: 'POST',
            data: data,
            dataType: 'application/json',
            success: callback,
            error: callback


        });
    },
};


document.onclick += Project.hideMenu;
