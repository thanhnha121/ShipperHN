// for btn search clicked, btn menu clicked
window.addEventListener("load", function () {
    // Set a timeout...
    window.scrollTo(0, 1);
});

$('.search-box,.menu').hide();
$('#location-nav').hide();

$('.options li:first-child').click(function () {
    $(this).toggleClass('active');
    $('.search-box').toggle();
    $('#txt-search').focus();
    $('.menu').hide();
    $('.options li:last-child').removeClass('active');
    $('.options li:first-child').html('<img src="Content/images/options-search-active.png">');
    $('.options li:last-child').html('<img src="Content/images/options-menu-not-active.png">');
    $('.options li:first-child').css('border-left', 'none');
    $('#search-result').fadeOut(100);

});
$('.options li:last-child').click(function () {
    $(this).toggleClass('active');
    $('.menu').toggle();
    $('.search-box').hide();
    $('.options li:first-child').removeClass('active');
    $('.options li:last-child').html('<img src="Content/images/options-menu-active.png">');
    $('.options li:first-child').html('<img src="Content/images/options-search-not-active.png">');
    $('.options li:first-child').css('border-left', 'none');
    $('#search-result').fadeOut(100);
});
$('.content').click(function () {
    $('.search-box,.menu').hide();
    $('.options li:last-child, .options li:first-child').removeClass('active');
    $('.options li:first-child').html('<img src="Content/images/options-search-not-active.png">');
    $('.options li:last-child').html('<img src="Content/images/options-menu-not-active.png">');
    $('.options li:first-child').css('border-left', '1px solid #aaaaaa');
    $('#search-result').fadeOut(100);
});

$('#txt-search').attr('autocomplete', 'off');

////////////////////////////////////////////////////
// SESSION DATA
///////////////////////////////////////////////////
var locationTimes = [
new Date(),
new Date(),
new Date(),
new Date(),
new Date(),
new Date(),
new Date(),
new Date(),
new Date(),
new Date(),
new Date(),
new Date(),
new Date()
];

var lastScrollTop = 0;
var isLoadTop = false;
var isLoadBottom = false;
var isEnd = false;
var isFilter = false;
var curLocation = "";
var locationIndex = 13;
var curTime = new Date();
var loadTopCount = 0;
var showTipsCount = 0;
var isSearching = false;

function BackToTop() {
    $('#btn-back-to-top').fadeOut();
    $("html,body").animate({ scrollTop: $('body').position().top });
    PauseScroll();
}

function PauseScroll() {
    $(window).unbind('scroll');
    setTimeout(function () {
        $(window).scroll(function () {
            ScrollFunc();
        });
    }, 1000);
}

function GetBottom() {
    if (!isLoadBottom && !isEnd) {
        $('#bottom-loading').fadeIn();
        isLoadBottom = true;
        var bottomUrl = document.getElementById("data-GetMorePosts").getAttribute("data-bottomUrl");
        var bottomTime = document.getElementById("data-GetMorePosts").getAttribute("data-bottomTime");
        var lastPostId = document.getElementById("data-GetMorePosts").getAttribute("data-lastPostId");
        $.ajax({
            type: "GET",
            url: bottomUrl,
            data: { bottomTime: bottomTime, lastPostId: lastPostId },
            cache: false,
            success: function (data) {
                $('#get-more-posts').append(data);
                $('#bottom-loading').fadeOut(1000);
                var updateUrl = document.getElementById("data-GetMorePosts").getAttribute("data-updateUrl");
                $.ajax({
                    type: "GET",
                    url: updateUrl,
                    data: { bottomTime: bottomTime, lastPostId: lastPostId },
                    cache: false,
                    success: function (data) {
                        var newBottomTime = data.split('@')[0];
                        var newLastPostId = data.split('@')[1];
                        var isBottom = data.split('@')[2];
                        document.getElementById("data-GetMorePosts").setAttribute("data-bottomTime", newBottomTime);
                        document.getElementById("data-GetMorePosts").setAttribute("data-lastPostId", newLastPostId);
                        if (isBottom === 'true') {
                            isEnd = true;
                            $('#bottom-loading').html('<div style="text-align: center;font-size: 14px; font-weight: bold">E N D</div>');
                            $('#bottom-loading').fadeIn();
                        }
                    },
                    error: function (xhr, status, error) {
                    }
                });
                isLoadBottom = false;
            },
            error: function (xhr, status, error) {
            }
        });
    }
}

function GetTop(isScroll) {
    if (!isLoadTop) {
        loadTopCount++;
        isLoadTop = true;
        var topUrl = document.getElementById("data-GetNewPosts").getAttribute("data-topUrl");
        var topTime = document.getElementById("data-GetNewPosts").getAttribute("data-topTime");
        var firstPostId = document.getElementById("data-GetNewPosts").getAttribute("data-firstPostId");
        $.ajax({
            type: "GET",
            url: topUrl,
            data: { topTime: topTime, firstPostId: firstPostId },
            cache: false,
            success: function (data) {
                $('#get-new-posts').prepend('<div id = "new-post-' + loadTopCount + '" '
                    + 'style="-webkit-animation: background 15s cubic-bezier(10,10,10,10) infinite; '
                    + 'animation: background 15s cubic-bezier(1,0,0,10) infinite;"'
                    + '></div>');
                $('#new-post-' + loadTopCount).html(data);
                var updateUrl = document.getElementById("data-GetNewPosts").getAttribute("data-updateUrl");
                var tmpLoadTopCount = loadTopCount;
                setTimeout(function () {
                    $('#new-post-' + tmpLoadTopCount).css('-webkit-animation', '');
                    $('#new-post-' + tmpLoadTopCount).css('animation', '');
                }, 6000);
                $.ajax({
                    type: "GET",
                    url: updateUrl,
                    data: { topTime: topTime, lastPostId: firstPostId },
                    cache: false,
                    success: function (data) {
                        var newBottomTime = data.split('@')[0];
                        var newLastPostId = data.split('@')[1];
                        document.getElementById("data-GetNewPosts").setAttribute("data-topTime", newBottomTime);
                        document.getElementById("data-GetNewPosts").setAttribute("data-firstPostId", newLastPostId);
                        if (isHideLocationBar === 'false') {
                            UpdateLocationNav();
                        }
                    },
                    error: function (xhr, status, error) {
                    }
                });
                isLoadTop = false;
            },
            error: function (xhr, status, error) {
            }
        });
    }
}

function ScrollFunc() {
    if ($(window).scrollTop() + $(window).height() > 30000) {
        $('#btn-back-to-top').fadeIn(1000);
    }
    if (!isFilter && !isSearching) {
        if ($(window).scrollTop() + $(window).height() > $(document).height() - 600) {
            GetBottom();
        }
    } else {
        if ($(window).scrollTop() + $(window).height() > $(document).height() - 600) {
            FilterByLocationGetBottom();
        }
        if ($(window).scrollTop() === 0) {
            loadTopCount++;
            $('#btn-back-to-top').fadeOut();
        }
        if (locationIndex < 13) {
            locationTimes[locationIndex] = new Date();
        }
    }
}

// scroll to end/top
$(window).scroll(function () {
    ScrollFunc();
});

function FilterByLocationGetTop(isScroll) {
    if (!isLoadTop) {
        isLoadTop = true;
        var filterByLocationGetTopUrl = document.getElementById("data-FilterByLocationGetTop").getAttribute("data-FilterByLocationGetTopUrl");
        var topTime = document.getElementById("data-GetNewPosts").getAttribute("data-topTime");
        var firstPostId = document.getElementById("data-GetNewPosts").getAttribute("data-firstPostId");
        $.ajax({
            type: "GET",
            url: filterByLocationGetTopUrl,
            data: { topTime: topTime, firstPostId: firstPostId, location: curLocation },
            cache: false,
            success: function (data) {
                $('#get-new-posts').prepend(data);
                var updateUrl = document.getElementById("data-FilterByLocationGetTop").getAttribute("data-updateUrl");
                $.ajax({
                    type: "GET",
                    url: updateUrl,
                    data: { topTime: topTime, lastPostId: firstPostId, location: curLocation },
                    cache: false,
                    success: function (data) {
                        var newBottomTime = data.split('@')[0];
                        var newLastPostId = data.split('@')[1];
                        document.getElementById("data-GetNewPosts").setAttribute("data-topTime", newBottomTime);
                        document.getElementById("data-GetNewPosts").setAttribute("data-firstPostId", newLastPostId);
                        UpdateLocationNav();
                    },
                    error: function (xhr, status, error) {
                    }
                });
                isLoadTop = false;
            },
            error: function (xhr, status, error) {
            }
        });
    }
}

function FilterByLocationGetBottom() {
    if (!isLoadBottom && !isEnd) {
        $('#bottom-loading').fadeIn();
        isLoadBottom = true;
        var filterByLocationGetBottomUrl = document.getElementById("data-FilterByLocationGetBottom").getAttribute("data-FilterByLocationGetBottomUrl");
        var bottomTime = document.getElementById("data-GetMorePosts").getAttribute("data-bottomTime");
        var lastPostId = document.getElementById("data-GetMorePosts").getAttribute("data-lastPostId");
        $.ajax({
            type: "GET",
            url: filterByLocationGetBottomUrl,
            data: { bottomTime: bottomTime, lastPostId: lastPostId, location: curLocation },
            cache: false,
            success: function (data) {
                $('#get-more-posts').append(data);
                $('#bottom-loading').fadeOut();
                var updateUrl = document.getElementById("data-FilterByLocationGetBottom").getAttribute("data-updateUrl");
                $.ajax({
                    type: "GET",
                    url: updateUrl,
                    data: { bottomTime: bottomTime, lastPostId: lastPostId, location: curLocation },
                    cache: false,
                    success: function (data) {
                        var newBottomTime = data.split('@')[0];
                        var newLastPostId = data.split('@')[1];
                        var isBottom = data.split('@')[2];
                        document.getElementById("data-GetMorePosts").setAttribute("data-bottomTime", newBottomTime);
                        document.getElementById("data-GetMorePosts").setAttribute("data-lastPostId", newLastPostId);
                        if (isBottom === 'true') {
                            isEnd = true;
                            $('#bottom-loading').html('<div style="text-align: center;font-size: 14px; font-weight: bold">E N D</div>');
                            $('#bottom-loading').fadeIn();
                        }
                    },
                    error: function (xhr, status, error) {
                    }
                });
                isLoadBottom = false;
            },
            error: function (xhr, status, error) {
            }
        });
    }
}

function FilterByLocation(location, index, locationNavContext) {
    isSearching = false;
    isEnd = false;
    $('#bottom-loading').css('display', 'none');
    var filterByLocationUrl = document.getElementById("data-FilterByLocation").getAttribute("data-FilterByLocationUrl");
    $.ajax({
        type: "GET",
        url: filterByLocationUrl,
        data: { location: location },
        cache: false,
        success: function (data) {
            $(locationNavContext).html(location);
            $('#get-new-posts').html('');
            $('#get-more-posts').html('');
            $('#post-list').html(data);
            window.ResetLoading();
            isFilter = true;
            var updateUrl = document.getElementById("data-FilterByLocation").getAttribute("data-updateUrl");
            $.ajax({
                type: "GET",
                url: updateUrl,
                data: { location: location },
                cache: false,
                success: function (data) {
                    var newLastPostId = data.split('@')[0];
                    var isBottom = data.split('@')[1];
                    document.getElementById("data-GetMorePosts").setAttribute("data-bottomTime", new Date());
                    document.getElementById("data-GetMorePosts").setAttribute("data-lastPostId", newLastPostId);
                    if (isBottom === 'true') {
                        isEnd = true;
                        $('#bottom-loading').html('<div style="text-align: center;font-size: 14px; font-weight: bold">E N D</div>');
                        $('#bottom-loading').fadeIn();
                    }
                },
                error: function (xhr, status, error) {
                }
            });
        },
        error: function (xhr, status, error) {
        }
    });
    if (index < 13) {
        locationIndex = index;
        locationTimes[index] = new Date();
    }
    curLocation = location;
    $("html,body").animate({ scrollTop: $('#location-nav').position().top });
}

function UpdateLocationNav() {
    var getNewLocationPostCountUrl = document.getElementById("data-GetNewLocationPostCount").getAttribute("data-GetNewLocationPostCountUrl");
    $.ajax({
        url: getNewLocationPostCountUrl,
        type: "GET",
        data: { locationsDatimes: locationTimes },
        cache: false,
        success: function (data) {
            $('#location-nav').html(data);
        },
        error: function (xhr, status, error) {
        }
    });
}

setInterval(function () {
    if (isHideLocationBar === 'false') {
        UpdateLocationNav();
    }
    if (isFilter) {
        FilterByLocationGetTop(false);
    } else {
        GetTop(false);
    }
}, 3000);

function LoginWithFacebook() {
    ShowAlert("Chức năng này đang được bảo trì!");
}

// on first load
function onload() {
    var value = $.cookie("isHideLocationBar");
    if (value === undefined) {
        isHideLocationBar = 'false';
    } else {
        isHideLocationBar = value;
    }
    if (isHideLocationBar === 'true') {
        $('#location-nav').hide();
        $('#hide-location-bar').html("Show Location Bar");
    } else {
        $('#location-nav').show();
        $('#hide-location-bar').html("Hide Location Bar");
    }

    //near by feature
    var value1 = $.cookie("isNearByOn");
    if (value1 === undefined) {
        isNearByOn = 'false';
    } else {
        isNearByOn = value1;
    }
    if (isNearByOn === 'true') {
        $('#nearby').fadeIn();
        $('#hide-nearby').html("Turn On Near By Feature");
    } else {
        $('#nearby').fadeOut();
        $('#hide-nearby').html("Turn Off Near By Feature");
    }

    PauseScroll();
    var preHeight = screen.height / 10 * 6 + 700;
    var preTop = screen.height / 30;
    document.getElementById('post-preview').style.top = preTop * 8 + "px";
    document.getElementById('post-preview').style.height = preHeight + "px";
    document.getElementById('post-preview-close').style.top = (preHeight + preTop * 9 + 20) + "px";
}

function Search() {
    var input = $('#txt-search').val();
    if (input.length < 30) {
        if (input.charAt(0) === '@' && input.length > 1) {
            var searchUserByNameUrl = document.getElementById("data-SearchUserByName").getAttribute("data-SearchUserByNameUrl");
            $.ajax({
                url: searchUserByNameUrl,
                type: "GET",
                data: { name: input.substr(1, input.length - 1) },
                cache: false,
                success: function (data) {
                    $('#search-result').html(data);
                    $('#search-result').fadeIn(100);
                },
                error: function (xhr, status, error) {
                }
            });
            return;
        }
        if (input.charAt(0) === '#' && input.length > 1) {
            if (input.length > 12) {
                ShowAlert("Số điện thoại không đúng định dạng!");
                return;
            }
            for (var i = 1; i < input.length; i++) {
                var integer = parseInt(input.charAt(i), 10);
                if (isNaN(integer) || input.charAt(i).toLowerCase() === 'e') {
                    ShowAlert("Số điện thoại không đúng định dạng!");
                    return;
                }
            }
            var searchUserByPhoneUrl = document.getElementById("data-SearchUserByPhone").getAttribute("data-SearchUserByPhoneUrl");
            $.ajax({
                url: searchUserByPhoneUrl,
                type: "GET",
                data: { phone: input.substr(1, input.length - 1) },
                cache: false,
                success: function (data) {
                    $('#search-result').html(data);
                    $('#search-result').fadeIn(100);
                },
                error: function (xhr, status, error) {
                }
            });
            return;
        } 
    } else {
        ShowAlert("Từ khóa quá dài (< 30 ký tự)");
    }
}

$('#txt-search').keyup(function () {
    Search();
});

function ShowAlert(message) {
    $('#alert').html(message);
    $('#alert').fadeIn(200);
    setTimeout(function () {
        $('#alert').fadeOut(200);
    }, 3000);
}

function GetAllPostsByUserId(userid) {
    var getAllPostsByUserIdUrl = document.getElementById("data-GetAllPostsByUserId").getAttribute("data-GetAllPostsByUserIdUrl");
    $.ajax({
        url: getAllPostsByUserIdUrl,
        type: "GET",
        data: { userid: userid },
        cache: false,
        success: function (data) {
            $('#search-result').css('display', 'none');
            window.ResetLoading();
            $('#post-list').html(data);
            $('#get-more-posts').html("");
            $('#get-new-posts').html("");
            isSearching = true;
        },
        error: function (xhr, status, error) {
        }
    });
}


// Update created time
setInterval(function () {
    $(".date-new").get().forEach(function (entry, index, array) {
        var curTime = new Date();
        var time = new Date($(array[index]).attr("data-Time"));
        var updateTime;
        var diff = parseInt(Math.abs(curTime - time) / 1000);
        if (diff <= 10) {
            updateTime = "Vừa xong";
        }
        else if (diff < 60 && diff > 10) {
            updateTime = parseInt(diff) + " giây trước";
        }
        else if (diff < 3600 && diff >= 60) {
            updateTime = parseInt(diff / 60) + " phút trước";
        }
        else if (diff < 3600 * 24) {
            updateTime = parseInt(diff / 3600) + " giờ trước";
        } else {
            updateTime = parseInt(diff / 3600 * 24) + " ngày trước";
        }
        $(array[index]).html(updateTime);
    });
    $(".date-old").get().forEach(function (entry, index, array) {
        var curTime = new Date();
        var time = new Date($(array[index]).attr("data-Time"));
        var updateTime;
        var diff = parseInt(Math.abs(curTime - time) / 1000);
        if (diff <= 10) {
            updateTime = "Vừa xong";
        }
        else if (diff < 60 && diff > 10) {
            updateTime = parseInt(diff) + " giây trước";
        }
        else if (diff < 3600 && diff >= 60) {
            updateTime = parseInt(diff / 60) + " phút trước";
        }
        else if (diff < 3600 * 24) {
            updateTime = parseInt(diff / 3600) + " giờ trước";
        } else {
            updateTime = parseInt(diff / 3600 * 24) + " ngày trước";
        }
        $(array[index]).html(updateTime);
    });

}, 10000);

// When the user clicks anywhere outside of the modal, close it
$('body').click(function (event) {
    $('.modal').hide();
});


//Hide location bar
var isHideLocationBar;
function HideLocationBar() {
    if (isHideLocationBar === 'false') {
        $('#location-nav').hide();
        $('#hide-location-bar').html("Show Location Bar");
        isHideLocationBar = 'true';
        $.cookie('isHideLocationBar', 'true');
    } else {
        $('#location-nav').show();
        $('#hide-location-bar').html("Hide Location Bar");
        isHideLocationBar = 'false';
        $.cookie('isHideLocationBar', 'false');
    }
}

//Near by feature
var isNearByOn;
function HideNearBy() {
    if (isNearByOn === 'true') {
        $('#nearby').fadeOut();
        $('#hide-nearby').html("Turn On Near By Feature");
        isNearByOn = 'false';
        $.cookie('isNearByOn', 'false');
    } else {
        geoFindMe();
        $('#nearby').fadeIn();
        $('#hide-nearby').html("Turn Off Near By Feature");
        isNearByOn = 'true';
        $.cookie('isNearByOn', 'true');
    }
}

function ResetLoading() {
    var loadingString = '<div class="paginate">'
        + '<div class="loading">'
        + '<div class="line1"></div>'
        + '<div class="line2"></div>'
        + '<div class="line3"></div>'
        + '<div class="line4"></div>'
        + '<div class="line5"></div>'
        + '</div>'
        + '</div>';
    $('#bottom-loading').html(loadingString);
}

