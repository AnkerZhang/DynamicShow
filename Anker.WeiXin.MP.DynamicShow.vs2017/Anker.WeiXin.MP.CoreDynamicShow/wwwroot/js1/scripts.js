// Designed and developed by Dandy Weng
jQuery(function () {
    var $document = $(document),
        $window = $(window),
        $html = $("html"),
        $body = (window.opera) ? (document.compatMode == "CSS1Compat" ? $("html") : $("body")) : $("html, body"),
        $header = $("#header"),
        $main = $("#main"),
        widgetCtr = "#widget-container",
        rootDir = "//blog.dandyweng.com/wp-content/themes/albatross/",
        delta = 0,
        screenWidth = screen.width,
        setDelta = function (e) {
            var h = $window.height();
            var y = e.clientY - h / 2;
            delta = y * 0.03;
        }

    $window.load(function () {
        $html.addClass(skel.vars.browser + " " + skel.vars.deviceType + " " + skel.vars.deviceType + skel.vars.deviceVersion);
        if (skel.vars.isTouch) $html.addClass("touch-device");
        $window
            .trigger("resize")
            .trigger("scroll");
    })
        .scroll(function () {
            if (!skel.isActive("narrow")) {
                var y = $window.scrollTop() / ($document.height() * 0.0045);
                if (y > 200) y = 200;
                $header.css("background-position", "0 " + (-1 * parseInt(y)) + "px");
            }
            //$("#comment-icon").css("margin", ($(window).scrollTop() + $(window).height() / 2) / $(document).height() * 1000 + "px auto");

            if (!skel.vars.isTouch) {
                var s = ($(window).scrollTop() + ($(window).height() / 2)) / $(document).height() * 100;
                if (s > 90) s = 90; if (s < 8) s = 8;
                $("#comment-icon").css("top", parseInt(s) + "%");
            }
            //console.log(parseInt($window.scrollTop()) +"/"+ parseInt(y));
        })
        .resize(function () {
            // Set The Size Of Header Background
            if (!skel.isActive("narrow")) {
                $header.css("background-size", "auto, auto " + ($window.height() + 200) + "px");
                dynamicFontsize();
            } else {
                $html.removeAttr("style");
            }

            // Set Thumb Sizes
            $("div.post-thumbnail").each(function () {
                var e = $(this),
                    h = e.prev("header").height();
                e.css("height", h);
            });
        });


    $document.pjax("a:not([data-ajax=false]):not([target=_blank])", "#main")
        .on("pjax:timeout", function (event) {
            event.preventDefault()
        })
        .on("pjax:start", function () {
            $("#comment-bar").fadeOut(300, function () {
                $(this).remove();
            })
        })
        .on("pjax:send", function () {
            NProgress.start();
            $main.fadeTo(500, 0);
        })
        .on("pjax:success", function () {
            $.ajax({
                url: window.location.href,
                data: "request=body-class",
                type: "GET",
                error: function (request) {
                    NProgress.done();
                },
                success: function (data) {
                    $("body").removeClass().addClass(data);
                    NProgress.done();
                }
            });
            $window.trigger("load").trigger("resize").trigger("scroll");
            $main.fadeTo(500, 1, function () {
                if ($window.scrollTop() > 20) {
                    var pos = location.hash ? location.hash.offset().top : 0;
                    $body.animate({ scrollTop: pos }, 100);
                }
            });
        })
        .on("click", ".switchboard li", function (e) {
            var t = e.target.nodeName;
            if (t == "LI" && $(this).children("a").length) $(this).find("a")[0].click();
        })
        .on("click", "[data-action=switch-font-type]", function () {
            $html.toggleClass("serif-font");
        })
        .on("click", "[data-action=switch-night-mode]", function () {
            $html.toggleClass("night-mode");
        })
        .on("click", "[data-action=switch-fullscreen-mode]", function () {
            $(this).find("i").toggleClass("fa-expand").toggleClass("fa-compress");
            if (!document.fullscreenElement && !document.mozFullScreenElement && !document.webkitFullscreenElement && !document.msFullscreenElement) {
                if (document.documentElement.requestFullscreen) {
                    document.documentElement.requestFullscreen();
                } else if (document.documentElement.msRequestFullscreen) {
                    document.documentElement.msRequestFullscreen();
                } else if (document.documentElement.mozRequestFullScreen) {
                    document.documentElement.mozRequestFullScreen();
                } else if (document.documentElement.webkitRequestFullscreen) {
                    document.documentElement.webkitRequestFullscreen(Element.ALLOW_KEYBOARD_INPUT);
                }
            } else {
                if (document.exitFullscreen) {
                    document.exitFullscreen();
                } else if (document.msExitFullscreen) {
                    document.msExitFullscreen();
                } else if (document.mozCancelFullScreen) {
                    document.mozCancelFullScreen();
                } else if (document.webkitExitFullscreen) {
                    document.webkitExitFullscreen();
                }
            }
        })
        .on("click", "[data-confirm]", function () {
            return confirm($(this).data("confirm"));
        })
        .on("submit", "#comment-form", function () {
            NProgress.start();
            var e = $(this),
                post_id = $("#singular").data("post-id"),
                button = $("button[name=submit]"),

                author = e.find("input[name=author]").val(),
                email = e.find("input[name=email]").val(),
                url = e.find("input[name=url]").val(),
                content = e.find("textarea").val();
                comment_parent = e.find("input[name=comment_parent]").val(),
                images = e.find("input[name=images]").val(),

            $.cookie("comment-author-name", author, { expires: 365, path: "/" });
            $.cookie("comment-author-email", email, { expires: 365, path: "/" });
            $.cookie("comment-author-url", url, { expires: 365, path: "/" });

            $.ajax({
                url: "/Home/Send",
                data: $(this).serialize(),
                type: "post",
                beforeSend: function (data) {
                    if (!$.trim(content)) return false;
                    button.html('<i class="fa fa-circle-o-notch fa-spin"></i>');
                },
                error: function (request) {
                    console.log("Comment Failed to Post: " + request.responseText);
                    button.html('<i class="fa fa-times"></i>').addClass("error");
                    NProgress.done();
                },
                success: function (data) {
                    console.log("Comment Posted.");
                    var parent_id = $($.parseHTML(data)).filter("section.new-comment").data("parent-id");
                    console.log("parent: " + parent_id);
                    if (parent_id == 0) {
                        if (post_id == 1008) {
                            $("#comment-form").after(data);
                        } else {
                            $("#comments").prepend(data);
                        }
                    } else {
                        $("#comment-" + parent_id).append(data);
                        $("#comment-form.comment-panel").slideUp(800);
                    }

                    NProgress.done();

                    button.html('<i class="fa fa-check"></i>').addClass("done");
                    e.find("textarea").val("");

                    $("section.new-comment").slideDown(800, function () {
                        if (parent_id == 0 && post_id == 1008) {
                            var pos = $("section.new-comment").offset().top - 25;
                            $body.animate({ scrollTop: pos }, 300);
                        }
                    });
                    location.reload();
                }
            }); // end Ajax
            return false;
        }) // end submit
        .on("click touchstart", "#comment-bar", function (e) {
            if ($(e.target).parent("a").hasClass("close")) return;
            $html.addClass("comment-bar-open");
        })
        .on("click touchstart", "[data-action=close-comment-bar], html.comment-bar-open #main", function () {
            $html.removeClass("comment-bar-open");
        })
        .on("click touchstart", "[data-action=launch-comment-modal]", function () {
            var post_id = $("#singular").data("post-id");
            $.get(rootDir + "ajax/get-comments.php?type=modal&id=" + post_id, function (result) {
                $("body").append(result);
                $main.addClass("comment-modal-engaging").fadeTo(800, .3);
                $("#comment-modal").fadeIn(500);
                /*
                $window.scroll(function(){
                    $("#main.comment-modal-engaging").click();
                });
                */
            });
        })
        .on("click touchend", "[data-action=close-comment-modal]", function () {
            $main.removeClass("comment-modal-engaging").fadeTo(800, 1);
            $("#comment-modal").fadeOut(300, function () {
                $(this).remove();
            });
        })
        /*
        .on("click touchend", "#main.comment-modal-engaging", function(){
            $main.removeClass("comment-modal-engaging").fadeTo(800, 1);
            $("#comment-modal").fadeOut(300, function() {
                $(this).remove();
            });
        })
        */
        .on("click", "[data-action=reply]", function () {
            moveCommentFormTo($(this).closest("section"), true);
        })
        .on("click", "[data-action=like]", function () {
            var e = $(this),
                comment_id = e.closest("section").data("comment-id");
            if (!e.hasClass("liked")) {
                $.ajax({
                    url: "/Home/Evaluate",
                    data: "action=like&id=" + comment_id,
                    type: "post",
                    beforeSend: function (data) {

                    },
                    error: function (request) {
                        console.log("Like Failed: " + request.responseText);
                    },
                    success: function (data) {
                        $("#comment-" + comment_id + "-likes").text(data);
                        e.addClass("liked");
                        $.cookie("comment-" + comment_id + "-liked", "1", { expires: 30 });
                        console.log("Comment Liked. Total: " + data);
                        $("#comment-37214-likes")
                    }
                }); // end Ajax
            }
        })
        .on("click", "[data-action=dislike]", function () {
            var e = $(this),
                comment_id = e.closest("section").data("comment-id");
            if (!e.hasClass("disliked")) {
                $.ajax({
                    url: "/Home/Evaluate",
                    data: "action=dislike&id=" + comment_id,
                    type: "post",
                    beforeSend: function (data) {

                    },
                    error: function (request) {
                        console.log("Dislike Failed: " + request.responseText);
                    },
                    success: function (data) {
                        /*
                        var likes = parseInt($("#comment-" + comment_id + "-likes").text());
                        var dislikes = parseInt(data);
                        var c = likes - dislikes;
                        $("#comment-" + comment_id + "-likes").text(c);
                        */
                        $("#comment-" + comment_id + "-likes").text(data);
                        e.addClass("disliked");
                        $.cookie("comment-" + comment_id + "-disliked", "1", { expires: 30 });
                        console.log("Comment Disliked. Total: " + data);
                    }
                }); // end Ajax
            }
        })
        .on("click", "article[data-post-id].child-clickable", function (event) {
            $(this).find("h2 a")[0].click();
            //$.pjax.click(event, {container: "#main"})
        })
        .on("click touchend", "#next-post", function () {
            $(this).find("a")[0].click();
        })
        .on("mousemove", widgetCtr, setDelta).on("blur mouseleave", widgetCtr, function () {
            delta = 0;
        })
        .on("mouseenter", "[data-scrolling=stop]", function () {
            $document.off("mousemove", widgetCtr);
            delta = 0;
        })
        .on("mouseleave", "[data-scrolling=stop]", function () {
            $document.on("mousemove", widgetCtr, setDelta)
        })
        .on("ready", function () {

            // Load Widgets If Needed
            if (screenWidth >= 1024 && !skel.isActive("narrow")) loadWidgets();

            // Widget Scrolling
            (function f() {
                if (delta && !skel.vars.isTouch) {
                    $(widgetCtr).scrollTop(function (i, v) {
                        return v + delta;
                    });
                }
                requestAnimationFrame(f);
            })();

            // Change Header Background
            function changeHeaderBackground() {
                var backgroundCounts = $header.data("background-counts"),
                    currentBackground = $header.data("current-background");

                if (!skel.isActive("narrow")) {
                    var nextBackground = currentBackground + 1;
                    if (nextBackground > backgroundCounts) nextBackground = 1;
                    $header.removeClass("background-n" + currentBackground).addClass("background-n" + nextBackground).data("current-background", nextBackground);
                    console.log("Changing header background to image No." + nextBackground);
                }
            }
            setInterval(changeHeaderBackground, 30000);

        }).on("ready pjax:end", function () {
            // Comments Bar
            if ($("#singular").length == 1) {
                var post_id = $("#singular").data("post-id");
                if (post_id != 1008 && post_id != 1325) {
                    $.get(rootDir + "ajax/get-comments.php?type=bar&id=" + post_id, function (result) {
                        $("body").append(result);
                        $("#comment-bar").fadeIn(500);
                    });
                }
                if (post_id == 1325) {
                    moveCommentFormTo($("#comment-" + $("#singular").data("reply-id")), false);
                }
            }

            // GET Gravatar
            $(".comment-panel input[name=email]").blur(function () {
                var email = $(this).val();
                var regex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
                if (regex.test(email)) {
                    $.get(rootDir + "ajax/get-gravatar.php?e=" + email, function (result) {
                        console.log("Gravatar Fetched.");
                        //$("#author-gravatar").before(result).remove();
                        $("#author-gravatar").attr("src", result);
                    });
                }
            });

            // Lazyload
            $("img[data-original]").lazyload({
                effect: "fadeIn"
            }).load(function () {
                // Lightbox
                if ($("#singular").length > 0) {
                    //$("#singular.post-"+ post_id).find("a").filter("[href$='.png'],[href$='.jpg'],[href$='.gif']").fluidbox({
                    $(this).parent("a").filter("[href$='.png'],[href$='.jpg'],[href$='.gif']").fluidbox({
                        closeTrigger: [{
                            selector: ".fluidbox-overlay",
                            event: "click"
                        }, {
                            selector: "window",
                            event: "scroll"
                        }, {
                            selector: 'document',
                            event: 'keyup',
                            keyCode: 27
                        }]
                    });
                }
                //console.log($(this).attr('src') + ' loaded');
            });;

            // QR Code
            $("#qr").qrcode({
                render: "canvas",
                height: 150,
                width: 150,
                text: window.location.href,
                correctLevel: 0
            });

            // Code Highlighting
            $("code").litelighter({
                style: $html.hasClass("night-mode") ? "dark" : "light"
            }).wrap("<pre></pre>");


            // Font Size
            dynamicFontsize = function () {
                var scaleSource = $body.width(),
                    scaleFactor = 0.0365,
                    maxScale = 72.5,
                    minScale = 58; //Tweak these values to taste

                var fontSize = scaleSource * scaleFactor; //Multiply the width of the body by the scaling factor:

                if (fontSize > maxScale) fontSize = maxScale;
                if (fontSize < minScale) fontSize = minScale; //Enforce the minimum and maximums

                $html.css('font-size', fontSize + '%');
            }

            if (!skel.isActive("narrow")) {
                dynamicFontsize();
            }
        }).on("pjax:end", function () {
            loadWidgetGallery();
            loadWidgetLocation();
            loadWidgetComments();
        });

    skel.on("-narrow", function () {
        if (!isWidgetsLoaded()) loadWidgets();
    });

    function moveCommentFormTo($section, animated) {
        var parent_id = $section.data("comment-id"),
            parent_author = $section.children("header").find("h3").text(),
            slideDuration = animated ? 800 : 0;

        $("#comment_parent").val(parent_id);
        $("#comment-form.comment-panel").appendTo($section).slideDown(slideDuration);
        $("textarea[name=comment]").attr("placeholder", "回复 @" + $.trim(parent_author) + "：").focus();
    }

    // Load Widgets
    function loadWidgets() {
        //$(widgetCtr).addClass("loading");
        var data = {
            w: screenWidth,
            os: skel.vars.deviceType
        }

       
    }

    function isWidgetsLoaded() {
        return $(widgetCtr).hasClass("loaded");
    }

    // Load Widget Gallery
    function loadWidgetGallery() {
        var suffix = "jpg",
            request_count = 18,
            widget = $("[data-widget=gallery]");
        container = $("#montage");

        $.getJSON("https://api.camarts.cn/photos/essentials.php?count=" + request_count + "&rate=5&tags=all&locations=china&format=json&suffix=" + suffix + "&jsoncallback=?", function (data) {
            container.addClass("ready").empty();
            $.each(data.items, function (i, item) {
                var camats_id = item.id;
                var a = $("<a/>").attr({ "href": "http://camarts.cn/" + camats_id, "target": "_blank", "rel": "external", "class": "2u", "data-location": item.location, "data-camarts-id": item.id, "data-camarts-title": item.title });
                $("<img/>").attr("src", item.url).appendTo("#montage").wrap(a);
                //if ( i == 3 ) return false;
            });
            console.log("Montage Loaded.");
        });
    }

    // Load Widget Location
    function loadWidgetLocation() {
        var widget = $("[data-widget=location]");

        $.get("https://www.dandyweng.com/api/location/get-latest.php", function (result) {
            widget.find("p").html('<i class="fa fa-map-marker"></i> ' + result);
            if (result != "广东省广州市") widget.addClass("traveling");
        });
    }

    // Load Widget Comments
    function loadWidgetComments() {
        var widget = $("[data-widget=comments]"),
            container = $("#comments-list");
        //widget.addClass("loading");

        $.get(rootDir + "ajax/get-widget-comments.php", function (result) {
            container.empty().append(result);
            //widget.removeClass("loading");
            $(".widget-comment-avatar").error(function () {
                $(this).attr("src", rootDir + "images/overlay.png");
            });
        });
    }
});