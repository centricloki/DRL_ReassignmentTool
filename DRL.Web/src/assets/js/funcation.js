
$(function () {
  $('.nav-icon').click(function () {
    $('body').toggleClass('menu-open');
    $('.nav-icon').toggleClass('active');
  });
  // Get the container element
  var btnContainer = document.getElementById("navList");

  if (btnContainer != null) {
    // Get all a with class="btnLi" inside the container
    var btns = btnContainer.getElementsByClassName("btnLi");

    // Loop through the links and add the active class to the current/clicked link
    for (var i = 0; i < btns.length; i++) {
      btns[i].addEventListener("click", function () {
        var current = document.getElementsByClassName("active");
        current[0].className = current[0].className.replace(" active", "");
        this.className += " active";
      });
    }
  }

});

// $(function () {
//   $('#CustomerReass').click(function (e) {
//     if (e.target.id == 'reassspan') {
//       if (document.getElementsByClassName("submenu")[0].style.display === "none" || document.getElementsByClassName("submenu")[0].style.display === "") {
//         document.getElementsByClassName("submenu")[0].style.display = "block";
//       } else {
//         document.getElementsByClassName("submenu")[0].style.display = "none";
//       }
//     }
//     else {
//       document.getElementsByClassName("submenu")[0].style.display = "block";
//     }
//   });
// });