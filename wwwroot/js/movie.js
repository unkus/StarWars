$('#movie-holder option').each(function() {
    console.log(this);
});
$('#movie-holder option').select();

$('#attach-button').click(function() {
    console.log(this);
});

$('#detach-button').click(function() {
    console.log(this);
});

function attachMovie() {
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
        id: "detach-button",
        class: "btn-sm border-0 btn-close",
        type: "button",
        onClick: "detachMovie(" + id + ")"
    });
    
    movieList.append(dd);
    dd.append(span);
    dd.append(button);
}

function detachMovie(id) {
    var movieHolder = $('#movie-holder');
    var movieSelector = $('#detached-movie-list');
    
    var selectedMovie = movieHolder.find("[value=" + id + "]");
    selectedMovie.appendTo(movieSelector);
    selectedMovie.select()

    $('dd[data-id=' + id + ']').remove();
}