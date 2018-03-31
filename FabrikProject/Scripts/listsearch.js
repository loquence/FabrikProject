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
         var randNum = rand * 10000;
         $(".stock-body").append("<tr>\n<td>" + stock + "</td>\n<td>" + ticker + "</td>\n<td><input type=\"text\" class=\"form-control quantity-box\" name=\"[" + numb + "].Quantity\" placholder=\"Quantity\" required/></td>\n<td><input type=\"date\" name=\"[" + numb + "].DatePrice\"class=\"form-control price-box\" required /></td>\n<td>$" + randNum + "</td>\n" + "</tr>\n<input type=\"hidden\" name=\"[" + numb + "].Stock\" value=\"" + ticker + "\" />\n<input type=\"hidden\" name=\"[" + numb + "].PriceWhenBought\" value=\"" + randNum + "\" />")
         numb++;

     });

    $('[data-toggle="popover"]').popover(); 

    $('#CancelButton').click(function () {
        $('.stock-body').empty();

    });


});


