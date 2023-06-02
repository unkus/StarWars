function detachMovie() {
    var dd = $(this).closest('dd');
    var text = dd.find('span').text();

    var movieHolder = $('#movie-holder');
    var movieList = $('#detached-movie-list');
    var movieInput = $('#selected-movie');

    var selectedOption = movieHolder.find('[value="' + text + '"]');
    selectedOption.selected = false;
    selectedOption.appendTo(movieList);

    dd.remove();
}

$('button[id=detach-movie-button]').each(function () {
    $(this).click(detachMovie);
});

$('#attach-movie-button').click(function () {
    var attachedMovies = $('#attached-movie-list');
    var movieHolder = $('#movie-holder');
    var detachedMovies = $('#detached-movie-list');
    var selectedMovie = $('#selected-movie');

    detachedMovies.find('[value="' + selectedMovie.val() + '"]').remove();
    
    var option = $('<option selected/>');
    option.prop('value', selectedMovie.val());
    movieHolder.append(option);

    var dd = $('<dd></dd>');
    
    var span = $('<span>' + selectedMovie.val() + '</span>');
    
    var button = $('<button></button>', {
        id: 'detach-movie-button',
        class: 'btn-sm border-0 btn-close',
        type: 'button'
    });
    button.on('click', detachMovie);

    dd.append(span);
    dd.append(button);
    attachedMovies.append(dd);
    
    selectedMovie.val('');
});
