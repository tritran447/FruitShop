    "use strict";


     /*--
        preloader
    -----------------------------------*/
    $(window).on("load", function () {
		$(".theme-loader").fadeOut(500);
	});

    /*--
        Header Sticky
    -----------------------------------*/

    window.onscroll = function () {
        const left = document.getElementById("header");

        if (left.scrollTop > 50 || self.pageYOffset > 50) {
            left.classList.add("sticky")
        } else {
            left.classList.remove("sticky");
        }
    }    


     /*--    
        Tabs
    -----------------------------------*/  
    const tabs = document.querySelectorAll('[data-tab-target]')
    const tabContents = document.querySelectorAll('.argani-tab-pane')

    tabs.forEach(tab => {
        tab.addEventListener('click', () => {
            const target = document.querySelector(tab.dataset.tabTarget)
            tabContents.forEach(tabContent => {
                tabContent.classList.remove('active')
            })
            tabs.forEach(tab => {
                tab.classList.remove('active')
            })
            tab.classList.add('active')
            target.classList.add('active')
        })
    })

    
    /*--
        Menu parent Element Icon
    -----------------------------------*/
    const $subMenu = document.querySelectorAll('.sub-menu');
    $subMenu.forEach(function (subMenu) {
        const menuExpand = document.createElement('span')
        menuExpand.classList.add('menu-icon')
        // menuExpand.innerHTML = '+'
        subMenu.parentElement.insertBefore(menuExpand, subMenu)
        if (subMenu.classList.contains('mega-menu')) {
            subMenu.classList.remove('mega-menu')
            subMenu.querySelectorAll('ul').forEach(function (ul) {
                ul.classList.add('sub-menu')
                const menuExpand = document.createElement('span')
                menuExpand.classList.add('menu-icon')
                menuExpand.innerHTML = '+'
                ul.parentElement.insertBefore(menuExpand, ul)
            })
        }
    })

    /*--
        Search Js
    -----------------------------------*/
	var $searchWrap = $('.search-wrap');
	var $navSearch = $('.search-btn');
	var $searchClose = $('#search-close');

	$('.search-btn').on('click', function (e) {
		e.preventDefault();
		$searchWrap.animate({ opacity: 'toggle' }, 500);
		$navSearch.add($searchClose).addClass("open");
	});

	$('.search-close').on('click', function (e) {
		e.preventDefault();
		$searchWrap.animate({ opacity: 'toggle' }, 500);
		$navSearch.add($searchClose).removeClass("open");
	});

	function closeSearch() {
		$searchWrap.fadeOut(200);
		$navSearch.add($searchClose).removeClass("open");
	}

	$(document.body).on('click', function (e) {
		closeSearch();
	});

	$(".search-btn, .main-search-input").on('click', function (e) {
		e.stopPropagation();
	});

        
    /*--
        Mobile Menu 
    -----------------------------------*/

    /* Get Sibling */
    const getSiblings = function (elem) {
        const siblings = []
        let sibling = elem.parentNode.firstChild
        while (sibling) {
            if (sibling.nodeType === 1 && sibling !== elem) {
                siblings.push(sibling)
            }
            sibling = sibling.nextSibling
        }
        return siblings
    }

    /* Slide Up */
    const slideUp = (target, time) => {
        const duration = time ? time : 500;
        target.style.transitionProperty = 'height, margin, padding'
        target.style.transitionDuration = duration + 'ms'
        target.style.boxSizing = 'border-box'
        target.style.height = target.offsetHeight + 'px'
        target.offsetHeight
        target.style.overflow = 'hidden'
        target.style.height = 0
        window.setTimeout(() => {
            target.style.display = 'none'
            target.style.removeProperty('height')
            target.style.removeProperty('overflow')
            target.style.removeProperty('transition-duration')
            target.style.removeProperty('transition-property')
        }, duration)
    }

    /* Slide Down */
    const slideDown = (target, time) => {
        const duration = time ? time : 500;
        target.style.removeProperty('display')
        let display = window.getComputedStyle(target).display
        if (display === 'none') display = 'block'
        target.style.display = display
        const height = target.offsetHeight
        target.style.overflow = 'hidden'
        target.style.height = 0
        target.offsetHeight
        target.style.boxSizing = 'border-box'
        target.style.transitionProperty = 'height, margin, padding'
        target.style.transitionDuration = duration + 'ms'
        target.style.height = height + 'px'
        window.setTimeout(() => {
            target.style.removeProperty('height')
            target.style.removeProperty('overflow')
            target.style.removeProperty('transition-duration')
            target.style.removeProperty('transition-property')
        }, duration)
    }

    /* Slide Toggle */
    const slideToggle = (target, time) => {
        const duration = time ? time : 500;
        if (window.getComputedStyle(target).display === 'none') {
            return slideDown(target, duration)
        } else {
            return slideUp(target, duration)
        }
    }


    /*--
		Offcanvas/Collapseable Menu 
	-----------------------------------*/
    const offCanvasMenu = function (selector) {

        const $offCanvasNav = document.querySelector(selector),
            $subMenu = $offCanvasNav.querySelectorAll('.sub-menu');
        $subMenu.forEach(function (subMenu) {
            const menuExpand = document.createElement('span')
            menuExpand.classList.add('menu-expand')
            // menuExpand.innerHTML = '+'
            subMenu.parentElement.insertBefore(menuExpand, subMenu)
            if (subMenu.classList.contains('mega-menu')) {
                subMenu.classList.remove('mega-menu')
                subMenu.querySelectorAll('ul').forEach(function (ul) {
                    ul.classList.add('sub-menu')
                    const menuExpand = document.createElement('span')
                    menuExpand.classList.add('menu-expand')
                    menuExpand.innerHTML = '+'
                    ul.parentElement.insertBefore(menuExpand, ul)
                })
            }
        })

        $offCanvasNav.querySelectorAll('.menu-expand').forEach(function (item) {
            item.addEventListener('click', function (e) {
                e.preventDefault()
                const parent = this.parentElement
                if (parent.classList.contains('active')) {
                    parent.classList.remove('active');
                    parent.querySelectorAll('.sub-menu').forEach(function (subMenu) {
                        subMenu.parentElement.classList.remove('active');
                        slideUp(subMenu)
                    })
                } else {
                    parent.classList.add('active');
                    slideDown(this.nextElementSibling)
                    getSiblings(parent).forEach(function (item) {
                        item.classList.remove('active')
                        item.querySelectorAll('.sub-menu').forEach(function (subMenu) {
                            subMenu.parentElement.classList.remove('active');
                            slideUp(subMenu)
                        })
                    })
                }
            })
        })
    }
    offCanvasMenu('.offcanvas-menu');



     /*--
        Countdown
    -----------------------------------*/
    function makeTimer($endDate, $this, $format) {
        var today = new Date();
        var BigDay = new Date($endDate),
          msPerDay = 24 * 60 * 60 * 1000,
          timeLeft = (BigDay.getTime() - today.getTime()),
          e_daysLeft = timeLeft / msPerDay,
          daysLeft = Math.floor(e_daysLeft),
          e_hrsLeft = (e_daysLeft - daysLeft) * 24,
          hrsLeft = Math.floor(e_hrsLeft),
          e_minsLeft = (e_hrsLeft - hrsLeft) * 60,
          minsLeft = Math.floor((e_hrsLeft - hrsLeft) * 60),
          e_secsLeft = (e_minsLeft - minsLeft) * 60,
          secsLeft = Math.floor((e_minsLeft - minsLeft) * 60);
    
        var yearsLeft = 0;
        var monthsLeft = 0
        var weeksLeft = 0;
    
        if ($format != 'short') {
          if (daysLeft > 365) {
            yearsLeft = Math.floor(daysLeft / 365);
            daysLeft = daysLeft % 365;
          }
    
          if (daysLeft > 30) {
            monthsLeft = Math.floor(daysLeft / 30);
            daysLeft = daysLeft % 30;
          }
          if (daysLeft > 7) {
            weeksLeft = Math.floor(daysLeft / 7);
            daysLeft = daysLeft % 7;
          }
        }
    
        var yearsLeft = yearsLeft < 10 ? "0" + yearsLeft : yearsLeft,
          monthsLeft = monthsLeft < 10 ? "0" + monthsLeft : monthsLeft,
          weeksLeft = weeksLeft < 10 ? "0" + weeksLeft : weeksLeft,
          daysLeft = daysLeft < 10 ? "0" + daysLeft : daysLeft,
          hrsLeft = hrsLeft < 10 ? "0" + hrsLeft : hrsLeft,
          minsLeft = minsLeft < 10 ? "0" + minsLeft : minsLeft,
          secsLeft = secsLeft < 10 ? "0" + secsLeft : secsLeft,
          yearsText = yearsLeft > 1 ? 'Years' : 'year',
          monthsText = monthsLeft > 1 ? 'Months' : 'month',
          weeksText = weeksLeft > 1 ? 'Weeks' : 'week',
          daysText = daysLeft > 1 ? 'Days' : 'day',
          hourText = hrsLeft > 1 ? 'Hours' : 'Hours',
          minsText = minsLeft > 1 ? 'Mints' : 'min',
          secText = secsLeft > 1 ? 'Secs' : 'sec';
    
        var $markup = {
          wrapper: $this.find('.countdown__item'),
          year: $this.find('.yearsLeft'),
          month: $this.find('.monthsLeft'),
          week: $this.find('.weeksLeft'),
          day: $this.find('.daysLeft'),
          hour: $this.find('.hoursLeft'),
          minute: $this.find('.minsLeft'),
          second: $this.find('.secsLeft'),
          yearTxt: $this.find('.yearsText'),
          monthTxt: $this.find('.monthsText'),
          weekTxt: $this.find('.weeksText'),
          dayTxt: $this.find('.daysText'),
          hourTxt: $this.find('.hoursText'),
          minTxt: $this.find('.minsText'),
          secTxt: $this.find('.secsText')
        }
    
        var elNumber = $markup.wrapper.length;
        $this.addClass('item-' + elNumber);
        $($markup.year).html(yearsLeft);
        $($markup.yearTxt).html(yearsText);
        $($markup.month).html(monthsLeft);
        $($markup.monthTxt).html(monthsText);
        $($markup.week).html(weeksLeft);
        $($markup.weekTxt).html(weeksText);
        $($markup.day).html(daysLeft);
        $($markup.dayTxt).html(daysText);
        $($markup.hour).html(hrsLeft);
        $($markup.hourTxt).html(hourText);
        $($markup.minute).html(minsLeft);
        $($markup.minTxt).html(minsText);
        $($markup.second).html(secsLeft);
        $($markup.secTxt).html(secText);
    }
    
    $('.countdown').each(function () {
        var $this = $(this);
        var $endDate = $(this).data('countdown');
        var $format = $(this).data('format');
        setInterval(function () {
          makeTimer($endDate, $this, $format);
        }, 0);
    });


    /*--
		Mousemove Parallax
	-----------------------------------*/
    var b = document.getElementsByTagName("BODY")[0];  

    b.addEventListener("mousemove", function(event) {
    parallaxed(event);

    });

    function parallaxed(e) {
        var amountMovedX = (e.clientX * -0.3 / 8);
        var amountMovedY = (e.clientY * -0.3 / 8);
        var x = document.getElementsByClassName("parallaxed");
        var i;
        for (i = 0; i < x.length; i++) {
            x[i].style.transform='translate(' + amountMovedX + 'px,' + amountMovedY + 'px)'
        }
    }

   /*--    
        Category Active
    -----------------------------------*/
    var swiper = new Swiper(".category-active", {
        slidesPerView: 4,
        spaceBetween: 30,
        loop: true,
        pagination: {
            el: ".category-active .swiper-pagination",
            clickable: true,
        },
        breakpoints: {
          0: {
            slidesPerView: 1,
          },
          576: {
            slidesPerView: 2,
            spaceBetween: 20,
          },
          768: {
            slidesPerView: 2,
          },
          992: {
            slidesPerView: 3,
          },
          1200: {
            slidesPerView: 4,
          },
        },
    });

    /*--    
        Product Active
    -----------------------------------*/
    var swiper = new Swiper(".product-active", {
        slidesPerView: 3,
        loop: true,
        navigation: {
          nextEl: '.product-active .swiper-button-next',
          prevEl: '.product-active .swiper-button-prev',
      },
        breakpoints: {
          0: {
            slidesPerView: 1,
          },
          576: {
            slidesPerView: 2,
          },
          768: {
            slidesPerView: 2,
          },
          992: {
            slidesPerView: 3,
          },
        },
    });

    /*--    
        Product two Active
    -----------------------------------*/
    var swiper = new Swiper(".product2-active .swiper-container", {
      slidesPerView: 4,
      spaceBetween: 30,
      loop: true,
      navigation: {
        nextEl: '.product2-active .swiper-button-next',
        prevEl: '.product2-active .swiper-button-prev',
    },
      breakpoints: {
        0: {
          slidesPerView: 1,
        },
        576: {
          slidesPerView: 2,
        },
        768: {
          slidesPerView: 3,
        },
        992: {
          slidesPerView: 4,
        },
      },
  });

   /*--    
        Product three Active
    -----------------------------------*/
    var swiper = new Swiper(".product3-active", {
      slidesPerView: 3,
      spaceBetween: 30,
      loop: true,
      navigation: {
        nextEl: '.product3-active .swiper-button-next',
        prevEl: '.product3-active .swiper-button-prev',
    },
      breakpoints: {
        0: {
          slidesPerView: 1,
        },
        576: {
          slidesPerView: 2,
        },
        768: {
          slidesPerView: 2,
        },
        992: {
          slidesPerView: 3,
        },
      },
  });


  /*--    
        Product four Active
    -----------------------------------*/
    var swiper = new Swiper(".product4-active .swiper-container", {
      slidesPerView: 4,
      loop: true,
      navigation: {
        nextEl: '.product4-active .swiper-button-next',
        prevEl: '.product4-active .swiper-button-prev',
    },
      breakpoints: {
        0: {
          slidesPerView: 1,
        },
        576: {
          slidesPerView: 2,
        },
        768: {
          slidesPerView: 3,
        },
        992: {
          slidesPerView: 4,
        },
      },
  });


  /*--    
        Product seven Active
    -----------------------------------*/
    var swiper = new Swiper(".product7-active .swiper-container", {
      slidesPerView: 6,
      spaceBetween: 30,
      loop: true,
      breakpoints: {
        0: {
          slidesPerView: 1,
        },
        576: {
          slidesPerView: 2,
        },
        768: {
          slidesPerView: 3,
        },
        992: {
          slidesPerView: 4,
        },
        1200: {
          slidesPerView: 6,
        },
      },
  });

    /*--    
        Brand Active
    -----------------------------------*/
    var swiper = new Swiper(".brand-active .swiper-container", {
        slidesPerView: 6,
        spaceBetween: 30,
        loop: true,
        navigation: {
            nextEl: '.brand-active .swiper-button-next',
            prevEl: '.brand-active .swiper-button-prev',
        },
        breakpoints: {
          0: {
            slidesPerView: 1,
          },
          576: {
            slidesPerView: 2,
            spaceBetween: 20,
          },
          768: {
            slidesPerView: 3,
          },
          992: {
            slidesPerView: 4,
          },
          1200: {
            slidesPerView: 6,
          },
        },
    });


     /*--
        Testimonial-7 Active
    -----------------------------------*/
    var swiper = new Swiper('.testimonial-active', {
      slidesPerView: 3,
      spaceBetween: 30,
      loop: true,
      pagination: {
        el: ".testimonial-active .swiper-pagination",
        clickable: true,
      },
      breakpoints: {
        0: {
          slidesPerView: 1,
        },
        576: {
          slidesPerView: 1,
        },
        768: {
          slidesPerView: 1,
        },
        992: {
          slidesPerView: 2,
        },
        1200: {
          slidesPerView: 3,
        }
      }
  });

  /*--
    Testimonial-8 Active
  -----------------------------------*/
    var swiper = new Swiper('.testimonial-2-active', {
      slidesPerView: 1,
      spaceBetween: 30,
      loop: true,  
      pagination: {
        el: ".testimonial-2-active .swiper-pagination",
        clickable: true,
      },      
  });

  /*--    
    swiper thumb gallery
  -----------------------------------*/ 

    var Nav = new Swiper(".gallery-nav .swiper-container", {
      spaceBetween: 8,
      slidesPerView: 4,
      loop: true,
      navigation: {
      nextEl: ".gallery-nav .swiper-button-next",
      prevEl: ".gallery-nav .swiper-button-prev",
      },
  });

  var gallery = new Swiper(".gallery .swiper-container", {
      spaceBetween: 1,
      spaceBetween: 0,
      loop: false,
      navigation: false,
      thumbs: {
      swiper: Nav,
      },
  });


    /*--    
        Counter Up
    -----------------------------------*/  

    $('.counter').counterUp({
      delay: 10,
      time: 1500,
    });


    /*--    
        slider Range
    -----------------------------------*/

      $("#slider-range").slider({
        range: true,
        min: 0,
        max: 800,
        values: [20, 450],
        slide: function (event, ui) {
            $("#amount1").val("$" + ui.values[0] + ".00");

            $("#amount2").val("$" + ui.values[1] + ".00");
        }
    });

    $("#amount1").val("$" + $("#slider-range").slider("values", 0) + ".00");

    $("#amount2").val("$" + $("#slider-range").slider("values", 1) + ".00");

    /*--
      Quantity plus minus
    ----------------------------------------- */
        $(".cart-plus-minus").prepend('<div class="dec qtybutton">-</div>');
        $(".cart-plus-minus").append('<div class="inc qtybutton">+</div>');
        $(".qtybutton").on("click", function() {
            var $button = $(this);
            var oldValue = $button.parent().find("input").val();
            if ($button.text() == "+") {
                var newVal = parseFloat(oldValue) + 1;
            } 
            else {
                if (oldValue > 0) {
                    var newVal = parseFloat(oldValue) - 1;
                } 
                else {
                    newVal = 0;
                }
            }
            $button.parent().find("input").val(newVal);
        });


    /*--
        AOS
    -----------------------------------*/   
    AOS.init({
        duration: 1200,
        once: true,
    });




