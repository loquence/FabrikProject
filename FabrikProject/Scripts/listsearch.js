 $(function (){
     $("#myInput").on("keyup", function () {
            var value = $(this).val().toLowerCase();
            $("#myList li").filter(function () {
                $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
            });
     });

     $(".stock li").click(function (e) {
         var txt = $(e.target).text();
         var rand = Math.random();
         $(".stock-body").append("<tr>\n<td>" +txt +"</td>\n<td>"+ txt + " Ticker</td>\n<td><input type=\"text\" class=\"form-control\" placholder=\"Quantity\" required/></td>\n<td><input type=\"date\" class=\"form-control\" required /></td>\n<td>$" +rand*10000+"</td>"+ "</tr>")


     });

     $('[data-toggle="popover"]').popover(); 


 });
