
/**
 * Get All Selected Values
 */
function getSelectValues(select) {
    
    if (typeof(select) === typeof("")) {
        select = document.getElementById(select);
    }

    var result = [];
    var options = select && select.options;
    var opt;

    for (var i = 0, iLen = options.length; i < iLen; i++) {
        opt = options[i];

        if (opt.selected) {
            result.push(opt.value || opt.text);
        }
    }
    return result;
}



function selectSort(SelList) {
    var ID = '';
    var Text = '';
    for (x = 0; x < SelList.length - 1; x++) {
        for (y = x + 1; y < SelList.length; y++) {
            if (SelList[x].text > SelList[y].text) {
                // Swap rows
                ID = SelList[x].value;
                Text = SelList[x].text;
                SelList[x].value = SelList[y].value;
                SelList[x].text = SelList[y].text;
                SelList[y].value = ID;
                SelList[y].text = Text;
            }
        }
    }
}

function selectFromTo(selectFrom, selectTo) {
    var SS1 = document.getElementById(selectFrom);
    var SS2 = document.getElementById(selectTo);

    var SelID = '';
    var SelText = '';
    // Move rows from SS1 to SS2 from bottom to top
    for (i = SS1.options.length - 1; i >= 0; i--) {
        if (SS1.options[i].selected == true) {
            SelID = SS1.options[i].value;
            SelText = SS1.options[i].text;
            var newRow = new Option(SelText, SelID);
            SS2.options[SS2.length] = newRow;
            SS1.options[i] = null;
        }
    }
    selectSort(SS2);
    return;

}

function getContextMenuPosition(event, contextMenu) {
    contextMenu = $(contextMenu);
    var mousePosition = {};
    var menuPosition = {};
    var menuDimension = {};

    menuDimension.x = contextMenu.outerWidth();
    menuDimension.y = contextMenu.outerHeight();
    mousePosition.x = event.pageX;
    mousePosition.y = event.pageY;

    if (mousePosition.x + menuDimension.x > $(window).width() + $(window).scrollLeft()) {
        menuPosition.x = mousePosition.x - menuDimension.x;
    } else {
        menuPosition.x = mousePosition.x;
    }

    if (mousePosition.y + menuDimension.y > $(window).height() + $(window).scrollTop()) {
        menuPosition.y = mousePosition.y - menuDimension.y;
    } else {
        menuPosition.y = mousePosition.y;
    }
    menuPosition.y += 2;
    menuPosition.x += 2;
    return menuPosition;
}


var ContextMenu = {
    MenuID: 'contextMenu',
    MenuOptions : [],
    // MenuCallback: null,
    Init: function (opts) {
        if (typeof (opts) === typeof ('') && opts === 'Admin-PhotoGallery') {
            ContextMenu.MenuOptions = [{
                text: 'Delete',
                callback: function (s, e) {
                    Content.Photo.Delete(s, s.title);
                }
            },
            {
                text: 'Display URL ',
                callback: function (s, e) {
                    alert(s.src);
                }
            }];

            

        }

        document.onclick = function (event) {
            ContextMenu.HideMenu(event);
        };
    },
    HideMenu: function (evt) {
        if (!$(evt.target).hasClass("context-menu")) {
            $("#" + ContextMenu.MenuID).remove();
        }
    },
    ShowMenu: function (evt) {
        evt = evt || window.event;
        var sender = evt.target;
        // check to see if the menu is already showing
        if ($("div.menu").length > 0) {
            return;
        }
        // 
        var div = document.createElement("div");
        div.setAttribute('id', ContextMenu.MenuID);
        

        // set the menu attributes
        var pos = ContextMenu.GetPosition(evt, div);
        div.setAttribute("class", "menu");
        div.setAttribute("style", "top:" + pos.y + "px;left:" + pos.x + "px;");

        for (var i = 0; i < ContextMenu.MenuOptions.length; i++) {
            var menuOption = ContextMenu.MenuOptions[i];
            var menuCallback = menuOption.callback;
            var divOption = document.createElement("div");
            $(divOption).addClass("content-menu-item");
            divOption.innerHTML = menuOption.text;
            divOption.title = i.toString();


            // attach click event
            if (divOption.addEventListener) {
                divOption.addEventListener('click', function (event) {
                    var index = parseInt(this.title);
                    ContextMenu.MenuOptions[index].callback(sender, event);
                    $(div).remove();
                });
            } else {
                divOption.attachEvent('click', function (event) {
                    var index = parseInt(this.title);
                    ContextMenu.MenuOptions[index].callback(sender, event);
                    $(div).remove();
                });
            }
            $(div).append(divOption);
        }
        
        var target = evt.target;
        if (target.tagName.toLowerCase() === 'img') {
            target = target.parentElement;
        }
        $(target).append(div);
        if (window.event) {
            window.event.returnValue = false;
        } else {
            evt.preventDefault();
        }
    },

    GetPosition: function (evt, contextMenu) {
        contextMenu = $(contextMenu);
        var mousePosition = {};
        var menuPosition = {};
        var menuDimension = {};

        menuDimension.x = contextMenu.outerWidth();
        menuDimension.y = contextMenu.outerHeight();
        mousePosition.x = evt.pageX;
        mousePosition.y = evt.pageY;

        if (mousePosition.x + menuDimension.x > $(window).width() + $(window).scrollLeft()) {
            menuPosition.x = mousePosition.x - menuDimension.x;
        } else {
            menuPosition.x = mousePosition.x;
        }

        if (mousePosition.y + menuDimension.y > $(window).height() + $(window).scrollTop()) {
            menuPosition.y = mousePosition.y - menuDimension.y;
        } else {
            menuPosition.y = mousePosition.y;
        }
        menuPosition.y += 2;
        menuPosition.x += 2;
        return menuPosition;
    }
};
