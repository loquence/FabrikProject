var numb = 0;
$(function () {

    $('#defaultOpen').click();

     $("#myInput").on("keyup", function () {
            var value = $(this).val().toLowerCase();
            $("#myList li").filter(function () {
                $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
            });
     });

     $(".stock li").click(function (e) {
         var check = $(e.target).parent();
         var stock;
         var ticker;
         if (check.is("li")) {
             ticker = check.find("h4").text();
             stock = check.find("p").text();
         }
         else {
             ticker = $(e.target).find("h4").text();
             stock = $(e.target).find("p").text();
         }
         
         var rand = Math.random();
         $(".stock-body").append("<tr>\n<td>" + stock + "</td>\n<td>" + ticker + "</td>\n<td><input type=\"text\" class=\"form-control quantity-box\" name=\"["+numb+"].Quantity\" placholder=\"Quantity\" required/></td>\n<td><input type=\"date\" name=\"["+numb+"].DatePrice\"class=\"form-control price-box\" required /></td>\n<td>$" + rand * 10000 + "</td>\n" + "</tr>\n<input type=\"hidden\" name\"["+numb+"].Stock\"value=\"" + ticker +"\" />")
         numb++;

     });

     $('[data-toggle="popover"]').popover(); 


});

function openStrategy(evt, strategyName) {
    // Declare all variables
    var i, tabcontent, tablinks;

    // Get all elements with class="tabcontent" and hide them
    tabcontent = document.getElementsByClassName("tabcontent");
    for (i = 0; i < tabcontent.length; i++) {
        tabcontent[i].style.display = "none";
    }

    // Get all elements with class="tablinks" and remove the class "active"
    tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active", "");
    }

    // Show the current tab, and add an "active" class to the button that opened the tab
    document.getElementById(strategyName).style.display = "block";
    evt.currentTarget.className += " active";
}
