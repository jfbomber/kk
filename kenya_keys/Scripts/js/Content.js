
var Content = {
    // private properties
    _ViewState: {
        ElementID: null,
        PreviousHtml: null
    },
    _ItemViewState: {

        ContentID: null,
        ItemID : null,
        ElementClass : ".item-edit",
        PreviousHtml : null
    },

    _LoadEditor: function () {
        if (window.location.toString().indexOf("somee") > 1) {
            CKEDITOR.basePath = "http://kenyakeys.somee.com/ckeditor/";
        }
        CKEDITOR.replace("_TextAreaHtml", {
            toolbar: 'Basic',
            extraPlugins: 'divarea',
            uiColor: '#9AB8F3'
        });
    },


    Photo : {


        Delete: function (sender, photoId) {
            if (confirm("Are you sure you want to delete this photo?")) {
                $.ajax({
                    url: "/Photo/Delete",
                    type :'POST',
                    data : { photoID : photoId },
                    success: function (response) {
                        var location = window.location;
                        window.location = location;
                        //window.location.reload();
                    }
                });
            };
        }
    },

    GetPhotoUrl: function (photoId, size) {
        size = size ? size : 'm';
        return "/Photo/GetPhoto?photoId="+photoId.toString()+"&size="+size;
    },

    // Sets the current content state to edit mode
    Edit  : function (contentId, elementId) {
        var element = document.getElementById(elementId);
        $.ajax({
            url: '/Content/Edit',
            data: { contentID: contentId },
            type : 'GET',
            success: function  d(html) {
                Content._ViewState.PreviousHtml = element.innerHTML;
                Content._ViewState.ElementID = elementId;
                element.innerHTML = html;
                Content._LoadEditor();
            }
        });
    },
    // Saves the current edit mode state
    Save: function (formName) {
        $("input[name=contentHtml]").val(escape(CKEDITOR.instances._TextAreaHtml.getData()));
        // we have to remove the html to allow post
        $("textarea[name=_TextAreaHtml]").val("");
        var form = $("form[name=" + formName + "]");
        var data = form.serialize();
        $.ajax({
            data: data,
            url : '/Content/Save',
            type: 'POST',
            success: function (response) {
                console.log(response);
                if (typeof (response) === typeof ("")) {
                    response = JSON.parse(response);
                }
                if (response.Result === 'success') {
                    var contentID = response.Data.ContentID;
                    window.location.reload();
                }
            }
        });

    },

    Item: {
        /**
         * Replaces the content with the default
         */
        Back: function () {
            // window.location.reload();
            var url = window.location.toString().split("?")[0];
            window.location = url;
        },

        Delete: function (itemID) {
            if (confirm("Are you sure you want to delete this item?")) {
                $.ajax({
                    url: '/Content/ItemDelete',
                    data: { itemID: itemID },
                    type: 'POST',
                    success: function (response) {
                        window.location.reload();
                    }
                });
            }
        },
        
        /**
         * Displays the list item
         * @param {Integer} ItemID, ContentID
         * @param {HtmlElement} Sender
         * @param {String} DisplayID, TitleID, HtmlID, PhotoViewerID
         */
        Display: function (options) {
            var ItemViewState = {};
            ItemViewState.Options = options;
            // this will be null if the display is loaded by the querystring
            if (options.Sender === null) {
                var tiles = $("div.tile-row div.tile");
                for (var i = 0; i < tiles.length; i++) {
                    var tile = tiles[i];
                    if (tile.title == options.ItemID) {
                        options.Sender = tile;
                        break;
                    }
                }
            }
            ItemViewState.Sender = options.Sender;
            ItemViewState.PhotoViewerElement = document.getElementById(options.PhotoViewerID);
            ItemViewState.TitleElement = document.getElementById(options.TitleID);
            ItemViewState.HtmlElement = document.getElementById(options.HtmlID);
            ItemViewState.DisplayElement = document.getElementById(options.DisplayID);
            ItemViewState.ContentID = options.ContentID;
            ItemViewState.ItemID = options.ItemID;

            Content._ItemViewState = ItemViewState;

            $.ajax({
                url: '/Content/GetContentItem',
                data: { itemID: options.ItemID },
                type: 'GET',
                success: function (item) {
                    
                    if (item) {
                        if (item.ItemPhotoID) {
                            document.getElementById(options.PhotoViewerID).style.display = "block";
                            document.getElementById(options.PhotoViewerID).src = Content.GetPhotoUrl(item.ItemPhotoID);
                        } 
                        // var decoded = $("<div/>").html(item.ItemHtml).text();
                        ItemViewState.HtmlElement.innerHTML = item.ItemHtml;
                        ItemViewState.TitleElement.innerHTML = "<div>" +
                                                                   "<div><a href=\"javascript:Content.Item.Back();\">Back</a></div>" +
                                                                   (item.IsAdmin ? "<div><a class=\"admin-edit\" href=\"javascript:Content.Item.Edit(" + options.ItemID.toString() + ", '" + options.DisplayID + "');\" ><img  src=\"/Images/icons/icon-add_20.png\" alt=\"Add Item [+]\" title=\"Add Item [+]\" />Edit Item</a>  |  " : "") +
                                                                   (item.IsAdmin ? "<a class=\"admin-edit\" href=\"javascript:Content.Item.Delete(" + options.ItemID.toString() + ");\" ><img  src=\"/Images/icons/icon-delete_20.png\" alt=\"Delete\" title=\"Delete\" />Delete Item</a></a></div> " : "") +
                                                                    "<h2>" + item.ItemTitle + "</h2>" +

                                                                "</div>";

                        $(".tile").removeClass("selected");
                        $(ItemViewState.Sender).addClass("selected");

                    }
                },
                error: function (err) {
                    console.log(err);
                }
            });


        },
        Edit: function (itemID, contentID) {
            // remove existing popups
            $(Content._ItemViewState.ElementClass).remove();
            // get the edit window
            $.ajax({
                url: '/Content/_ItemEdit',
                type : 'GET',
                data : { 
                    contentID : contentID,
                    itemID : itemID
                },
                success: function (html) {
                    $("body").append(html);
                    Content._ItemViewState.ContentID = contentID; 
                    Content._ItemViewState.ItemID = itemID;
                    Content._LoadEditor();
                }
            });
        },

        CancelEdit : function () {
            $(Content._ItemViewState.ElementClass).remove();
        },
        Save: function (formName) {
            $("input[name=itemHtml]").val(escape(CKEDITOR.instances._TextAreaHtml.getData()));
            // we have to remove the html to allow post
            $("textarea[name=_TextAreaHtml]").val("");
            var form = $("form[name=" + formName + "]");
            var data = form.serialize();
            $.ajax({
                data: data,
                url : '/Content/ItemSave',
                type: 'POST',
                success: function (response) {
                    console.log(response);
                    if (typeof (response) === typeof ("")) {
                        response = JSON.parse(response);
                    }
                    if (response.Result === 'success') {
                        window.location.reload()
                    }
                }
            });
        }
    }
};
