jQuery(document).ready(function($) {

  // Get comment count - http://help.disqus.com/customer/portal/articles/1131783-tutorial-get-comment-counts-with-the-api
  var disqusPublicKey = "K2pNITrezEXpslXodVpr9ei36ptwFgDWCm3wOBZZlDVBjSRI2Kpz1g316deeG1r0";
  var disqusShortname = "jonathanchannonblog"; 

  var urlArray = [];
  //urlArray.push('link:'+'http://blog.jonathanchannon.com/2012/12/19/why-use-nancyfx/');
    
  $('.commentcount').each(function () {
	var url = $(this).attr('data-disqus-url');
	urlArray.push('link:' + url);
  });
    
  if (urlArray.length > 0) {
	  $.ajax({
	    type: 'GET',
	    url: "https://disqus.com/api/3.0/threads/set.json",
	    data: { api_key: disqusPublicKey, forum : disqusShortname, thread : urlArray },
	    cache: false,
	    dataType: 'json',
	    success: function (result) {
	      for (var i in result.response) {

	        var countText = " comments";
	        var count = result.response[i].posts;

	        if (count == 1)
	          countText = " comment";
	             
	        $('div[data-disqus-url="' + result.response[i].link + '"]').html('<span class="link-comments">'+count + countText+'</span>');
	        //$('<span class="link-comments">'+count + countText+'</span>').insertAfter('.post-date');
	      }
	    }
	  });
	}

// Remove links outline in IE 7
	$("a").attr("hideFocus", "true").css("outline", "none");

// style Select, Radio, Checkbox
	if ($("select").hasClass("select_styled")) {
		cuSel({changedEl: ".select_styled", visRows: 10});
	}
	if ($("div,p").hasClass("input_styled")) {
		$(".input_styled input").customInput();
	}

// centering dropdown submenu (not mega-nav)
	$(".dropdown > li:not(.mega-nav)").hover(function(){
		var dropDown = $(this).children("ul");
		var dropDownLi = $(this).children().children("li").innerWidth();
		var posLeft = ((dropDownLi - $(this).innerWidth())/2);
		dropDown.css("left",-posLeft);		
	});	
// resonsive megamenu			
	var screenRes = $(window).width();   
	
	if (screenRes > 750) {				
		mega_show();		
    } 		
	
	function mega_show(){		
		$('.dropdown li').hoverIntent({
			sensitivity: 5,
			interval: 50, 
			over: subm_show, 
			timeout: 0, 
			out: subm_hide
		});
	}
	function subm_show(){	
		if ($(this).hasClass("parent")) {
			$(this).addClass("parentHover");
		};		
		$(this).children("ul.submenu-1").fadeIn(50);		
	}
	function subm_hide(){ 
		$(this).removeClass("parentHover");
		$(this).children("ul.submenu-1").fadeOut(50);		
	}
		
	$(".dropdown ul").parent("li").addClass("parent");
	$(".dropdown li:first-child, .pricing_box li:first-child, .sidebar .widget-container:first-child, .f_col .widget-container:first-child").addClass("first");
	$(".dropdown li:last-child, .pricing_box li:last-child, .widget_twitter .tweet_item:last-child, .sidebar .widget-container:last-child, .widget-container li:last-child").addClass("last");
	$(".dropdown li:only-child").removeClass("last").addClass("only");	
	$(".sidebar .current-menu-item, .sidebar .current-menu-ancestor").prev().addClass("current-prev");				
	
// tabs		
	if ($("ul").hasClass("tabs")) {		
		$("ul.tabs").tabs("> .tabcontent", {tabs: 'li', effect: 'fade'});	
	}
	if ($("ul").is(".tabs.linked")) {		
		$("ul.tabs").tabs("> .tabcontent");
	}
	
// odd/even
	$("ul.recent_posts > li:odd, ul.popular_posts > li:odd, .styled_table table>tbody>tr:odd, .boxed_list > .boxed_item:odd, .grid_layout .post-item:odd").addClass("odd");
	$(".widget_recent_comments ul > li:even, .widget_recent_entries li:even, .widget_twitter .tweet_item:even, .widget_archive ul > li:even, .widget_categories ul > li:even, .widget_nav_menu ul > li:even, .widget_links ul > li:even, .widget_meta ul > li:even, .widget_pages ul > li:even, .service_list .service_item:even").addClass("even");
	
// cols
	$(".row .col:first-child").addClass("alpha");
	$(".row .col:last-child").addClass("omega"); 	

// toggle content
	$(".toggle_content").hide(); 	
	$(".toggle").toggle(function(){
		$(this).addClass("active");
		}, function () {
		$(this).removeClass("active");
	});
	
	$(".toggle").click(function(){
		$(this).next(".toggle_content").slideToggle(300,'easeInQuad');
	});


// buttons	
	$(".button, .button_styled, .btn, .contact-social img, .btn-submit, .sign_up a").hover(function(){
		$(this).stop().animate({"opacity": 0.85});
	},function(){
		$(this).stop().animate({"opacity": 1});
	});


// Smooth Scroling of ID anchors	
  function filterPath(string) {
  return string
    .replace(/^\//,'')
    .replace(/(index|default).[a-zA-Z]{3,4}$/,'')
    .replace(/\/$/,'');
  }
  var locationPath = filterPath(location.pathname);
  var scrollElem = scrollableElement('html', 'body');
 
  $('a[href*=#].anchor').each(function() {
    $(this).click(function(event) {
    var thisPath = filterPath(this.pathname) || locationPath;
    if (  locationPath == thisPath
    && (location.hostname == this.hostname || !this.hostname)
    && this.hash.replace(/#/,'') ) {
      var $target = $(this.hash), target = this.hash;
      if (target && $target.length != 0) {
        var targetOffset = $target.offset().top;
          event.preventDefault();
          $(scrollElem).animate({scrollTop: targetOffset}, 400, function() {
            location.hash = target;
          });
      }
    }
   });	
  });
 
  // use the first element that is "scrollable"
  function scrollableElement(els) {
    for (var i = 0, argLength = arguments.length; i <argLength; i++) {
      var el = arguments[i],
          $scrollElement = $(el);
      if ($scrollElement.scrollTop()> 0) {
        return el;
      } else {
        $scrollElement.scrollTop(1);
        var isScrollable = $scrollElement.scrollTop()> 0;
        $scrollElement.scrollTop(0);
        if (isScrollable) {
          return el;
        }
      }
    }
    return [];
  }
  
	// prettyPhoto lightbox, check if <a> has atrr data-rel and hide for Mobiles
	if($('a').is('[data-rel]') && screenRes > 600) {
        $('a[data-rel]').each(function() {
			$(this).attr('rel', $(this).data('rel'));
		});
		$("a[rel^='prettyPhoto']").prettyPhoto({social_tools:false});	
    }
  
});

$(window).load(function() {
// mega dropdown menu	
	$('.dropdown .mega-nav > ul.submenu-1').each(function(){  
		var liItems = $(this);
		var Sum = 0;
		var liHeight = 0;
		if (liItems.children('li').length > 1){
			$(this).children('li').each(function(i, e){
				Sum += $(e).outerWidth(true);
			});
			$(this).width(Sum);
			liHeight = $(this).innerHeight();	
			$(this).children('li').css({"height":liHeight-30});					
		}
		var posLeft = 0;
		var halfSum = Sum/2;
		var screenRes = $(window).width();	
		if (screenRes > 960) {
			var mainWidth = 940; // width of main container to fit in.
		} else {
			var mainWidth = 744; // for iPad.
		}
		var parentWidth = $(this).parent().width();			
		var margLeft = $(this).parent().position();		
		margLeft = margLeft.left;		
		var margRight = mainWidth - margLeft - parentWidth;		
		var subCenter = halfSum - parentWidth/2;						
		if (margLeft >= halfSum && margRight >= halfSum) {			
			liItems.css("left",-subCenter);
		} else if (margLeft<halfSum) {
			liItems.css("left",-margLeft-1);
		} else if (margRight<halfSum) {
			posLeft = Sum - margRight - parentWidth - 10;
			liItems.css("left",-posLeft);					
		}
	});	
});