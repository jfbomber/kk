
var paging = {
    IMAGE_URL: "/Images/gallery/gallery_page.png",
    IMAGE_SELECTED_URL: "/Images/gallery/gallery_page_selected.png",
    selectedIndex: 0,
    dataSource: JSON.parse(document.getElementById('slide-dataSource').value).items,
    /*
     * Selects an images
     * {Image} image
     */
    selectPage: function (image) {
        $("div.gallery_paging img").attr("src", paging.IMAGE_URL);
        $(image.src ? image : image.target).attr("src", paging.IMAGE_SELECTED_URL);
        // get index

        var index = parseInt(image.alt ? image.alt : image.target.alt);
        paging.selectedIndex = index;
        // get no
        var note = $(".gallery_note")[0].innerHTML = unescape(paging.dataSource[index].description);
        var gallery = $($(".gallery")[0]).css('backgroundImage', "url('" + paging.dataSource[index].image + "')");
    },
};

window.onload = function () {
    // load images 
    paging.images = $("div.gallery_paging img");
    // add click event to the images
    $("div.gallery_paging img").click(paging.selectPage);

    setInterval(function () {
        var index = paging.selectedIndex + 1;
        if (index >= paging.dataSource.length) {
            index = 0;
        }
        paging.selectPage($("div.gallery_paging img")[index]);
    }, 10000);
}
