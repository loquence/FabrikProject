 $(function (){
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
         $(".stock-body").append("<tr>\n<td>" + stock + "</td>\n<td>" + ticker + " Ticker</td>\n<td><input type=\"text\" class=\"form-control\" placholder=\"Quantity\" required/></td>\n<td><input type=\"date\" class=\"form-control\" required /></td>\n<td>$" + rand * 10000 + "</td>" + "</tr>")


     });

     $('[data-toggle="popover"]').popover(); 


 });
