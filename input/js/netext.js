$( document ).ready( function() {
    prePrettifyer.setup();
    //tagCloud.setSize();
    //tagCloud.enableMultiTags();
} );

( function( tagCloud, $, undefined ) {
    tagCloud.maxSize = 60;
    tagCloud.minSize = 20;

    tagCloud.setSize = function() {
        var tags = $( 'a.tag' );

        var minCount, maxCount;
        tags.each( function() {
            var count = parseInt( $( this ).attr( 'data-postcount' ) );
            if ( isNaN( count ) ) {
                count = 1;
            }
            if ( !minCount || minCount > count ) {
                minCount = count;
            }
            if ( !maxCount || maxCount < count ) {
                maxCount = count;
            }
        } );

        var deltaDiff = ( tagCloud.maxSize - tagCloud.minSize ) / ( maxCount - minCount );

        tags.each( function() {
            var tag = $( this );
            var normalizedCount = parseInt( tag.attr( 'data-postcount' ) ) / minCount;
            var newSize = normalizedCount * deltaDiff - deltaDiff + tagCloud.minSize;
            tag.css( 'font-size', newSize + 'px' );
        } );
    };

    tagCloud.enableMultiTags = function() {
        $( 'a.tag' ).click( function() {
            var tag = $( this );
            if ( tag.hasClass( 'tag-selected' ) ) {
                tag.removeClass( 'tag-selected' ).addClass( 'tag-unselected' );
            } else if ( tag.hasClass( 'tag-unselected' ) ) {
                tag.removeClass( 'tag-unselected' ).addClass( 'tag-selected' );
            }

            getPosts();

            return false;
        } );
    };

    function getPosts() {
        var tags = [];
        $( 'a.tag.tag-selected' ).each( function() {
            tags.push( $( this ).html() );
        } );

        $.getJSON( '/api/blog/posts/tags/' + tags.join( "," ), function( posts ) {
            $( '#mainsection' ).empty();
            $.each( posts, function( i, post ) {
                var blogUrl = '/blog/post/' + post.Url.UrlFriendlyTitle;
                var postDiv = $( '<div/>' ).addClass( "post" );
                postDiv.append( $( '<h1/>' ).addClass( "post-title" ).html( $( '<a/>' ).attr( 'href', blogUrl ).text( post.Record.Title ) ) );
                postDiv.append( $( '<div/>' ).addClass( "post-author" ).text( post.User.UserName + ' - ' + new Date( post.Record.Created ).toDateString() ) );
                postDiv.append( $( '<div/>' ).addClass( "post-content" ).html( post.Record.Content ) );
                postDiv
                    .append(
                        $( '<div/>' )
                            .addClass( "comment-count" )
                            .html(
                                $( '<a/>' )
                                    .attr( 'href', blogUrl + "#disqus_tread" )
                                    .text( "Click here to see comments" ) ) );
                $( '#mainsection' ).append( postDiv );
            } );
            $( "code" ).addClass( "prettyprint linenums" );
            prettyPrint();
        } );
    }


}( tagCloud = window.tagCloud || {}, jQuery ) );

( function( prePrettifyer, $, undefined ) {
    prePrettifyer.setup = function() {
        $( 'table.pre' ).wrap('<div class="code-wrapper"></div>');
    };
}( prePrettifyer = window.prePrettifyer || {}, jQuery ) );