import 'bootstrap';

import {$, jQuery} from 'jquery';

import 'bootstrap/scss/bootstrap.scss';
import 'font-awesome/scss/font-awesome.scss';

import "../styles/app.scss";

jQuery(function () {
  // custom js for interactive elements in hippo

  // application view: environment show/hide tabs
  $('.tabs li:first-child a').addClass('is-active');
  $('.tab-pane').removeClass("is-active");
  $('.tab-pane:first').addClass('is-active');
  $('a.env-block').click(function(){
    var activeTab = $(this).attr('asp-id');

    $('.tabs li a').removeClass('is-active');
    $('.tab-pane').removeClass("is-active");
    $(this).addClass('is-active');
    $("[asp-class=" + activeTab + "]").addClass('is-active');

    return false;
  });

  // app view: expand and collapse revision metadata
  $('.rev-item .fa').click(function(){
    $(this).toggleClass('fa-chevron-down');
    $(this).toggleClass('fa-chevron-up');
    $(this).next().toggle();
  });


  // environment form: update labels
  $.fn.rewordLabels = function() {
    $('input[value="UseRangeRule"]').next("p").html("<strong>Auto-deploy</strong> <small>Deploy versions as they are created.</small>");
    $('input[value="UseSpecifiedRevision"]').next("p").html("<strong>Selective deploy</strong> <small>Lock deployment to a specific version.</small>");
  }

  // environment form: enable fancy radio buttons
  $.fn.deployLabels = function() {
    if ($('input.env-radio').is(':checked')) {
      // style the radio buttons accordingly
      $('.env-deploy-label').removeClass('is-active');
      $('input.env-radio:checked').parent('label').toggleClass('is-active');
    } else {
      // if nothing is selected, choose auto
      $('input[name=RevisionSelectionStrategy][value="UseRangeRule"]').prop('checked',true);

      $.fn.deployLabels();
    }
  };

  // channel form: show deploy config based on radio buttons
  $.fn.deploySelection = function() {
    if ($('input[value="UseRangeRule"]').is(':checked')) {
      $("#envManualField").addClass("hide");
      $("#envAutoField").removeClass("hide");
    };
    if ($('input[value="UseSpecifiedRevision"]').is(':checked')) {
      $("#envAutoField").addClass("hide");
      $("#envManualField").removeClass("hide");
    };
  }

  // channel form: show/hide env vars
  $("#domainToggle").bind("click", function() {
    $("#domain").toggleClass("hide");
  });

  if ($("input.env-radio").length > 0){
    $.fn.rewordLabels();
    $.fn.deployLabels();
    $.fn.deploySelection();

    $('input.env-radio').change(function() {
      $.fn.deployLabels();
      $.fn.deploySelection();
      // return false;
    })
  };
})
