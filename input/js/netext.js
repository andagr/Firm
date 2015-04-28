$( document ).ready( function() {
    prePrettifyer.setup();
    tagCloud.setSize();
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


}( tagCloud = window.tagCloud || {}, jQuery ) );

( function( prePrettifyer, $, undefined ) {
    prePrettifyer.setup = function() {
        $( 'table.pre' ).wrap('<div class="code-wrapper"></div>');
    };
}( prePrettifyer = window.prePrettifyer || {}, jQuery ) );