jQuery(function () {
  // custom js for interactive elements in hippo

  // application view environment show/hide tabs
  $('.tabs li:first-child a').addClass('is-active');
  $('.tab-pane').removeClass("is-active");
  $('.tab-pane:first').addClass('is-active');

  // Click function
  $('.tabs li a.env-block').click(function(){
    var activeTab = $(this).attr('asp-id');
    // alert("a tab was clicked with Env ID " + activeTab);
    $('.tabs li a').removeClass('is-active');
    $('.tab-pane').removeClass("is-active");
    $(this).addClass('is-active');
    // alert("show the content for " + activeTab + ".tab-pane");
    $("[asp-class=" + activeTab + "]").addClass('is-active');

    return false;
  });

  // show/hide env vars form
  $("#envVarToggle").bind("click", function() {
    $("#envVars").toggleClass("hide");
  });

  // environment create/edit form
  $.fn.deployReveal = function(){
    if ($("#envAuto").is(':checked')) {
      $("#envManualField").addClass("hide");
      $("#envAutoField").toggleClass("hide");
    }
    if ($("#envManual").is(':checked')) {
      $("#envAutoField").addClass("hide");
      $("#envManualField").toggleClass("hide");
    }
  }

  $.fn.deployReveal();

  $('.env-new-form input.env-radio').change(function() {
    $.fn.deployReveal();
  });
})
