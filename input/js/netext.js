$( document ).ready( function() {
    prePrettifyer.setup();
    tagCloud.setSize();
    tags.filter();
} );

(function(tags, $, undefined) {
    tags.filter = function() {
        var search = location.search;
        if (search && search.length > 1) {
            var qp = search.substr(1).split("&");
            var tag = qp.map(function(el) { 
                          return el ? el.split("=") : null; }
                      ).filter(function(el) {
                          return el && el.length == 2 && el[0] == "tag"; })
                      .map(function(el) {
                          return el[1];
                      });
            if (tag && tag.length > 0) {
                tag = decodeURIComponent(tag);
                $(".post").each(function() {
                    var $this = $(this);
                    var postTags = $this.data("tags") || "";
                    if (postTags.split(",").every(function(el) { return el != tag })) {
                        $this.hide();
                    }
                })
            }
        }
    }
}(tags = window.tags || {}, jQuery));

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