$(document).ready(function () {
  var location = $('#location-div').attr('data-location');
  var list = $('.location-nav ul li');
  for (var i = 0; i < list.length; i++) {
    if (list.eq(i).find('.current').text().trim() == location) {
      list.eq(i).find('.selected').show();
    }
  }
  $('body').keyup(function (e) {
    if (e.which === 27) {
      $('#search-result').hide();
    }
  });
});

// for btn search clicked, btn menu clicked
window.addEventListener("load", function () {
  // Set a timeout...
  window.scrollTo(0, 1);
});

$('.options li:first-child').click(function () {
  $(this).toggleClass('active');
  $('.search-box').toggle();
  $('#txt-search').focus();
  $('.menu').hide();
  $('.options li:last-child').removeClass('active');
  $('.options li:first-child').html('<img src="Content/images/options-search-active.png">');
  $('.options li:last-child').html('<img src="Content/images/options-menu-not-active.png">');
  $('.options li:first-child').css('border-left', 'none');
  $('#search-result').hide();

});
$('.options li:last-child').click(function () {
  $(this).toggleClass('active');
  $('.menu').toggle();
  $('.search-box').hide();
  $('.options li:first-child').removeClass('active');
  $('.options li:last-child').html('<img src="Content/images/options-menu-active.png">');
  $('.options li:first-child').html('<img src="Content/images/options-search-not-active.png">');
  $('.options li:first-child').css('border-left', 'none');
  $('#search-result').hide();
});
$('.content').click(function () {
  $('.search-box,.menu').hide();
  $('.options li:last-child, .options li:first-child').removeClass('active');
  $('.options li:first-child').html('<img src="Content/images/options-search-not-active.png">');
  $('.options li:last-child').html('<img src="Content/images/options-menu-not-active.png">');
  $('.options li:first-child').css('border-left', '1px solid #aaaaaa');
  $('#search-result').hide();
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

var UrlLoadMorePost = '/Post/LoadMorePost';
var UrlLoadPrePost = '/Post/LoadPrePost';
var UrlSearch = '/Post/Search';
var UrlLoadNoti = '/Post/LoadNoti';

function BackToTop() {
  $('#btn-back-to-top').hide();
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

var isLoadingBottom = false;
var isLoadingTop = false;
function ScrollFunc() {
  if ($(window).scrollTop() + $(window).height() > 30000) {
    $('#btn-back-to-top').show();
  }
  if ($(window).scrollTop() + $(window).height() > $(document).height() - 600) {
    if (!isLoadingBottom) {
      $('#bottom-loading').show();
      LoadBottom();
    }
  }
  if ($(window).scrollTop() === 0) {
    $('#btn-back-to-top').hide();
    if (!isLoadingTop) {
      $('#top-loading').show();
      LoadTop();
    }
  }
}

function hideLoadingTop() {
  setTimeout(() => {
    $('#top-loading').hide();
  }, 200);
}

function hideLoadingBottom() {
  setTimeout(() => {
    $('#bottom-loading').hide();
  }, 200);
}

function LoadBottom() {
  isLoadingBottom = true;
  var list = $('#post-list article');
  var lastTime = list.eq(list.length - 1).attr('data-time');
  var location = $('#location-div').attr('data-location');
  var listIds = [];
  for (var i = list.length - 30; i < list.length; i++) {
    if (i >= 0) {
      listIds.push(list.eq(i).attr('data-id') - 0);
    }
  }
  $.ajax({
    url: UrlLoadMorePost,
    type: "POST",
    data: { lastTime: lastTime, location: location, listIds: listIds },
    success: function (res) {
      $('#post-list').append(res);
      isLoadingBottom = false;
      hideLoadingBottom();
    },
    error: function (xhr, status, error) {
      console.log(xhr);
      console.log(status);
      console.log(error);
    }
  });
}

function LoadTop() {

  isLoadingTop = true;
  var list = $('#post-list article');
  var topTime = list.eq(0).attr('data-time');
  var location = $('#location-div').attr('data-location');
  var listIds = [];
  for (var i = 0; i < list.length && i < 30; i++) {
    listIds.push(list.eq(i).attr('data-id') - 0);
  }
  $.ajax({
    url: UrlLoadPrePost,
    type: "POST",
    data: { topTime: topTime, location: location, listIds: listIds },
    success: function (res) {
      $('#post-list').prepend(res);
      isLoadingTop = false;
      hideLoadingTop();
    },
    error: function (xhr, status, error) {
      console.log(xhr);
      console.log(status);
      console.log(error);
    }
  });
}

// scroll to end/top
$(window).scroll(function () {
  ScrollFunc();
});

function getLocationIndex(lo) {
  switch (lo) {
    case 'ba đình': return 0;
    case 'hoàn kiếm': return 1;
    case 'tây hồ': return 2;
    case 'long biên': return 3;
    case 'cầu giấy': return 4;
    case 'đống đa': return 5;
    case 'hai bà trưng': return 6;
    case 'hoàng mai': return 7;
    case 'thanh xuân': return 8;
    case 'hà đông': return 9;
    case 'bắc từ liêm': return 10;
    case 'nam từ liêm': return 11;
    case 'ngoại thành': return 12;
    default:
      return -1;
  }
}

var isLoadingFilter = false;
function FilterByLocation(location, index, e) {
  if (isLoadingFilter) {
    return;
  }
  isLoadingFilter = true;
  if (index === -1) {
    locationTimes[getLocationIndex(location)] = new Date();
  } else if (index < 13) {
    locationTimes[index] = new Date();
  }
  var date = new Date();
  var lastTime = date.toString().substring(0, 24);

  $.ajax({
    url: UrlLoadMorePost,
    type: "POST",
    data: { lastTime: lastTime, location: location, listIds: [0] },
    success: function (res) {
      $('#post-list').html(res);

      $('#location-div').attr('data-location', location);
      $('#location-nav ul li .selected').hide();
      var li;
      if ($(e).hasClass('current') || $(e).hasClass('count')) {
        li = $(e).parent('li');
      } else if ($(e).hasClass('count')) {
        li = $(e).parent('span').parent('li');
      } else {
        li = $(e);
      }
      if (index === -1) {
        var list = $('#location-nav ul li');
        for (var i = 0; i < list.length; i++) {
          if (list.eq(i).find('.current').text().trim().includes(location)) {
            li = list.eq(i);
            break;
          }
        }
      }
      li.find('.selected').show();
      li.find('span .count').html('');
      window.history.pushState('', 'ShipperHN - ' + '\"' + location + '\"', location.split(' ').join('-'));
      $("html,body").animate({ scrollTop: $('#location-nav').position().top });
      isLoadingFilter = false;
    },
    error: function (xhr, status, error) {
      console.log(xhr);
      console.log(status);
      console.log(error);
    }
  });
}

function UpdateLocationNav() {
  var data = [];
  for (var i = 0; i < locationTimes.length; i++) {
    data.push(locationTimes[i].toString().substring(0, 24));
  }
  $.ajax({
    url: UrlLoadNoti,
    type: "POST",
    data: { districts: data },
    success: function (res) {
      var split = res.split(',');
      var listE = $('.location-nav ul li');
      var count = 0;
      for (var j = 0; j < split.length; j++) {
        if (split[j] !== '0') {
          listE.eq(j).find('.count').html('(' + split[j] + ')');
          count += (split[j] - 0);
        }
      }
      if (count > 0) {
        document.title = '(' + count + ')' + " ShipperHN";
      }
    },
    error: function (xhr, status, error) {
      console.log(xhr);
      console.log(status);
      console.log(error);
    }
  });
}

setInterval(function () {
  if (isHideLocationBar === 'false') {
    UpdateLocationNav();
  }
  LoadTop();
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
    $('#nearby').show();
    $('#hide-nearby').html("Turn On Near By Feature");
  } else {
    $('#nearby').hide();
    $('#hide-nearby').html("Turn Off Near By Feature");
  }

  PauseScroll();
}

function Search() {
  var input = $('#txt-search').val().trim();
  if (input.length < 30 && input.length > 0) {
    $.ajax({
      url: UrlSearch,
      type: "POST",
      data: { input: input },
      success: function (res) {
        $('#search-result').html(res);
        $('#search-result').show();
      },
      error: function (xhr, status, error) {
        console.log(xhr);
        console.log(status);
        console.log(error);
      }
    });
  } else if (input.length > 30) {
    ShowAlert("Từ khóa quá dài, tối đa 30 ký tự!");
  }
}

$('#txt-search').keyup(function (e) {
  if (e.which !== 27) {
    Search();
  }
});

function ShowAlert(message) {
  $('#alert').html(message);
  $('#alert').show();
  setTimeout(function () {
    $('#alert').hide();
  }, 3000);
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
      updateTime = parseInt(diff / 3600 / 24) + " ngày trước";
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
      updateTime = parseInt(diff / 3600 / 24) + " ngày trước";
    }
    $(array[index]).html(updateTime);
  });

}, 10000);

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
    $('#nearby').hide();
    $('#hide-nearby').html("Turn On Near By Feature");
    isNearByOn = 'false';
    $.cookie('isNearByOn', 'false');
  } else {
    //    geoFindMe();
    $('#nearby').show();
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

