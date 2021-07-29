"use strict";

jQuery(function () {
  // custom js for interactive elements in hippo
  // application view environment show/hide tabs
  $('.tabs li:first-child a').addClass('is-active');
  $('.tab-pane').removeClass("is-active");
  $('.tab-pane:first').addClass('is-active');
  $('a.env-block').click(function () {
    var activeTab = $(this).attr('asp-id');
    $('.tabs li a').removeClass('is-active');
    $('.tab-pane').removeClass("is-active");
    $(this).addClass('is-active');
    $("[asp-class=" + activeTab + "]").addClass('is-active');
    return false;
  });
  $('.rev-item .fa').click(function () {
    $(this).toggleClass('fa-chevron-down');
    $(this).toggleClass('fa-chevron-up');
    $(this).next().toggle();
  }); // show/hide env vars form

  $("#envVarToggle").bind("click", function () {
    $("#envVars").toggleClass("hide");
  }); // environment create/edit form

  $.fn.deployReveal = function () {
    if ($("input.env-radio").is(':checked')) {
      // style the radio buttons accordingly
      $(".env-deploy-label").removeClass("is-active");
      $("input.env-radio:checked").parent("label").toggleClass("is-active");
    }
  }; // update radio button text labels


  $.fn.radioLabels = function () {
    $('input[value="UseRangeRule"]').next("p").html("<strong>Auto-deploy</strong> <small>Deploy versions as they are created.</small>");
    $('input[value="UseSpecifiedRevision"]').next("p").html("<strong>Selective deploy</strong> <small>Lock deployment to a specific version.</small>");
  }; // show form fields based on selection


  $.fn.radioSelection = function () {
    if ($('input[value="UseRangeRule"]').is(':checked')) {
      $("#envManualField").addClass("hide");
      $("#envAutoField").toggleClass("hide");
    }

    ;

    if ($('input[value="UseSpecifiedRevision"]').is(':checked')) {
      $("#envAutoField").addClass("hide");
      $("#envManualField").toggleClass("hide");
    }

    ;
  };

  $.fn.radioLabels();
  $.fn.deployReveal();
  $.fn.radioSelection();
  $('input.env-radio').change(function () {
    $.fn.deployReveal();
    $.fn.radioSelection();
    return false;
  });
});