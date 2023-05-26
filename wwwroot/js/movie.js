function detachMovie () {
    var dd = $(this).closest('dd');
    var id = dd.data()['id'];
    var movieHolder = $('#movie-holder');
    var movieSelector = $('#detached-movie-list');
    
    var selectedMovie = movieHolder.find("[value=" + id + "]");
    selectedMovie.appendTo(movieSelector);
    selectedMovie.select();

    dd.remove();
}

$('button[id=detach-movie-button]').each(function () {
    $(this).click(detachMovie);
});

$('#attach-movie-button').click(function() {
    var movieList = $('#attached-movie-list');
    var movieHolder = $('#movie-holder');
    var movieSelector = $('#detached-movie-list');

    var selectedMovie = movieSelector.find(":selected");
    var id = movieSelector.val();

    selectedMovie.appendTo(movieHolder);

    var dd = $( "<dd></dd", {
        "data-id": id
    });
    
    var span = $("<span>" + selectedMovie.text() + "</span>");

    var button = $("<button></button>", {
        id: "detach-movie-button",
        class: "btn-sm border-0 btn-close",
        type: "button"
    });
    button.on('click', detachMovie);
    
    dd.append(span);
    dd.append(button);
    movieList.append(dd);
});
