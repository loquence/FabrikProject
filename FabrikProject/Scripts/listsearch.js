var numb = 0;
var apiKey = 'BECFPZPO9R3JIDAN';
$(function () {

    $('#defaultOpen').click();

     $("#myInput").on("keyup", function () {
            var value = $(this).val().toLowerCase();
            $("#myList li").filter(function () {
                $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1);
            });
     });

     $(".stock li").click(function (e) {
         var check = $(e.target).parent();
         var stock;
         var ticker;
         var price;
         if (check.is("li")) {
             ticker = check.find("h4").text();
             stock = check.find("p").text();
         }
         else {
             ticker = $(e.target).find("h4").text();
             stock = $(e.target).find("p").text();
         }
         var url = 'https://www.alphavantage.co/query?function=BATCH_STOCK_QUOTES&symbols=' + ticker + '&apikey=' + apiKey;
         $.ajax({
             url: url,
             dataType: 'json',
             contentType: "application/json",
             success: function (data) {
                 price = data['Stock Quotes']['0']['2. price'];
                 console.log(price);
                 console.log(data['Stock Quotes']['0']['2. price']);
                 var rand = Math.random();
                 var randNum = rand * 10000;
                 $(".stock-body").append("<tr>\n<td>" + stock + "</td>\n<td>" + ticker + "</td>\n<td class=\"custom-td\"><div class=\"input-group custom-table-div\"><span class=\"input-group-addon\">#</span>\n<input type=\"number\" class=\"form-control currency\" name=\"[" + numb
                     + "].Quantity\" step=\"0.01\" data-number-to-fixed=\"2\" data-number-stepfactor=\"100\" placholder=\"Quantity\" required/></div></td>\n<td class=\"custom-td-pps\"><div class=\"input-group custom-table-div\"><span class=\"input-group-addon\">$</span><input type=\"number\" name=\"[" + numb +
                     "].SharePrice\" step=\"any\" class=\"form-control\" required /></div></td>\n<td class=\"custom-td-cm\"><div class=\"input-group custom-table-div\"><span class=\"input-group-addon\">$</span><input type=\"number\" step=\"any\" name=\"[" + numb +
                     "].InitialInvestment\" class=\"form-control\" required /></div></td>\n<td class=\"\"><div class=\"input-group custom-table-div\"><span class=\"input-group-addon\"><i class=\"glyphicon glyphicon-calendar\"></i></span><input type=\"Date\" name=\"[" + numb +
                     "].DatePurchased\" class=\"form-control\" required /></div></td>\n" + "</tr>\n<input type=\"hidden\" name=\"[" + numb + "].AssetTicker\" value=\"" + ticker +
                     "\" />\n<input type=\"hidden\" name=\"[" + numb +
                     "].AssetName\" value=\"" + stock + "\" />");
                 numb++;
                 
             }
         });
         
         

     });

    $('[data-toggle="popover"]').popover(); 

    $('#CancelButton').click(function () {
        $('.stock-body').empty();

    });

    


});


