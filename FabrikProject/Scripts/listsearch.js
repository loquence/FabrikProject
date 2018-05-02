var numb = 0;
var apiKey = 'BECFPZPO9R3JIDAN';
var ch = 0;
$(function () {
    $('.iicalc').on('keyup',function () {

        var price = $("#price").val();
        console.log(price);
        var quantity = $("#quantity").val();
        console.log(quantity);
        var initinv = price * quantity;
        $('#iinvest').html(initinv);
        $('#investment').val(initinv);
    })

    var drew = false;
    var search = $('#searchform');
    search.on('focusout', function (e) {
        $('.autocomplete-items').empty();
    });
    search.on("keyup focusin",function (e) {
        e.preventDefault();
        var x = $('#search-input').val();
        
        if (x == "") {
            $('.autocomplete-items').empty();
        }
        
        else {
            $.ajax({
                type: search.attr('method'),
                url: search.attr('action'),
                data: search.serialize(),
                success: function (data) {


                    var val1 = data["String"];
                    var val2 = data["slist"];
                    $('.autocomplete-items').empty();




                    if (val1 != null) {
                        /* datalist.empty();
                         console.log(val1);
                         datalist.append('<option value="' + val1 + '...">');*/
                        $('.autocomplete-items').empty();

                        $('.autocomplete-items').append("<div>narrow your search</div>");
                    }
                    else {
                        $('.autocomplete-items').empty();
                        for (var i = 0; i < val2.length; i++) {
                            $('.autocomplete-items').append('<div onclick="search()"><a href="/Account/AddStock?assetname=' + val2[i]["AssetName"] + '&assetticker=' + val2[i]["AssetTicker"] + '&assettype=' + val2[i]['AssetType'] + '">' + val2[i]["AssetName"] + ' - ' + val2[i]["AssetTicker"] + '</a>' + '</div>');
                        }

                    }


                }
            });
        }
        
    });

     $('#defaultOpen').click();

     $("#myInput").on("keyup focusin", function () {
            
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
                 var rand = Math.random();
                 var randNum = rand * 10000;
                 $(".stock-body").append("<tr>\n<td>" + stock + "</td>\n<td>" + ticker + "</td>\n<td class=\"custom-td\"><div class=\"input-group custom-table-div\"><span class=\"input-group-addon\">#</span>\n<input type=\"number\" class=\"form-control custom-add-form\" name=\"[" + numb
                     + "].Quantity\" step=\"0.01\" data-number-to-fixed=\"2\" data-number-stepfactor=\"100\" placholder=\"Quantity\" required/></div></td>\n<td class=\"custom-td-pps\"><div class=\"input-group custom-table-div\"><span class=\"input-group-addon\">$</span><input type=\"number\" name=\"[" + numb +
                     "].SharePrice\" step=\"any\" class=\"form-control\" required /></div></td>\n<td class=\"custom-td-cm\"><div class=\"input-group custom-table-div\"><span class=\"input-group-addon\">$</span><input type=\"number\" step=\"any\" id=\"commi\" name=\"[" + numb +
                     "].InitialInvestment\" class=\"form-control custom-add-form\" required /></div></td>\n<td class=\"custom-td-cm\"><div class=\"input-group custom-table-div\"><span class=\"input-group-addon\">$</span><input type=\"number\" step=\"any\" id=\"datepurch\" name=\"[" + numb +
                     "].Commissions\" class=\"form-control custom-add-form\" required /></div></td>\n<td class=\"\"><div class=\"input-group custom-table-div\"><span class=\"input-group-addon\"><i class=\"glyphicon glyphicon-calendar\"></i></span><input type=\"Date\" name=\"[" + numb +
                     "].DatePurchased\" class=\"form-control custom-add-form\" required /></div></td>\n" + "</tr>\n<input type=\"hidden\" id =\"tick\" name=\"[" + numb + "].AssetTicker\" value=\"" + ticker +
                     "\" />\n<input type=\"hidden\" id =\"aname\" name=\"[" + numb +
                     "].AssetName\" value=\"" + stock + "\" />");
                 numb++;
                 
             
         
         

     });
    document.getElementById("defaultOpen").click();
    $('[data-toggle="popover"]').popover(); 

    $('#CancelButton').click(function () {
        $('.stock-body').empty();

    });

    $('.partialContents').each(function (index, item) {
        var url = $(item).data("url");
        if (url && url.length > 0) {
            $(item).load(url);
        }
    });
    var test = $('#myInput');
    $('#myInput').on('keyup focusin', function () {
        var x = $('#search-multi').val();

        if (x == "") {
            $('.stock').empty();
        }
        
        else {
            $.ajax({
                type: test.attr('method'),
                url: test.attr('action'),
                data: test.serialize(),
                success: function (data) {

                    console.log(data);
                    var val1 = data["String"];
                    var val2 = data["slist"];
                    $('.stock').empty();

                    console.log(val1);
                    console.log(val2);

                    if (val1 != null) {
                        
                         
                         
                        $('.stock').empty();

                        $('.stock').append("<div>narrow your search</div>");
                    }
                    else {
                        $('.stock').empty();
                        for (var i = 0; i < val2.length; i++) {

                            $('.stock').append('<li class="list-group-item" onclick="addToTable(this)"><h4 class="list-group-item-heading" >' + val2[i]['AssetTicker'] + '</h4><p class="list-group-item-text" >' + val2[i]['AssetType'] +' - '+ val2[i]['AssetName'] + '</p></li>' );
                        }

                    }


                }
            });
        }
    });

    


});


function addToTable(e) {
    //var check = $(e.target).parent();
    var stock = $(e).find('p').text();
    var ticker = $(e).find('h4').text();
    //var price = $(e).find('p').text();
    
    /*if (check.is("li")) {
        ticker = check.find("h4").text();
        stock = check.find("p").text();
    }
    else {
        ticker = $(e.target).find("h4").text();
        stock = $(e.target).find("p").text();
    }*/
    //var url = 'https://www.alphavantage.co/query?function=BATCH_STOCK_QUOTES&symbols=' + ticker + '&apikey=' + apiKey;
    //var rand = Math.random();
    //var randNum = rand * 10000;
    $(".stock-body").append("<tr class=\""+numb +"\">\n<td>" + stock + "</td>\n<td>" + ticker + "</td>\n<td class=\"custom-td\"><div class=\"input-group custom-table-div\"><span class=\"input-group-addon\">#</span>\n<input type=\"number\" class=\"form-control\" name=\"[" + numb
        + "].Quantity\" step=\"0.01\" data-number-to-fixed=\"2\" data-number-stepfactor=\"100\" placholder=\"Quantity\" id=\"quant\" required/></div></td>\n<td class=\"custom-td-pps\"><div class=\"input-group custom-table-div\"><span class=\"input-group-addon\">$</span><input type=\"number\" id=\"sharep\" name=\"[" + numb +
        "].SharePrice\" step=\"any\" class=\"form-control\" required /></div></td>\n<td class=\"custom-td-cm\"><div class=\"input-group custom-table-div\"><span class=\"input-group-addon\">$</span><input type=\"number\" step=\"any\" id=\"ii\"name=\"[" + numb +
        "].InitialInvestment\" class=\"form-control \" required /></div></td>\n<td class=\"custom-td-cm\"><div class=\"input-group custom-table-div\"><span class=\"input-group-addon\">$</span><input type=\"number\" step=\"any\" id=\"commi\" name=\"[" + numb +
        "].Commissions\" class=\"form-control \" required /></div></td>\n<td class=\"\"><div class=\"input-group custom-table-div\"><span class=\"input-group-addon\"><i class=\"glyphicon glyphicon-calendar\"></i></span><input type=\"Date\" id=\"datep\" name=\"[" + numb +
        "].DatePurchased\" class=\"form-control \" required /></div></td>\n" + "<td><button class=\"butt btn btn-default glyphicon glyphicon-trash\" type=\"button\" onclick=\"remove('"+numb+"')\"></button></td>\n<input type=\"hidden\" id =\"tick\" name=\"[" + numb + "].AssetTicker\" value=\"" + ticker +
        "\" />\n<input type=\"hidden\" name=\"[" + numb +
        "].AssetName\"  id=\"aname\" value=\"" + stock + "\" /></tr>");
    numb++;
    $('#subBut').removeAttr('disabled');
}

function search(e) {
    var asset = $(e).find('.asset').text();
    var type = $(e).find('.assettype').text();
    
    $.get('/Account/AddStock', { assetticker: asset, assettype: type });

}

function remove(i) {

    
    var cl = '.' + i;
    var l = parseInt(i);
    
    
    $(cl).remove();
    for (r = l + 1;r < numb; r++) {
        var lc = '.' + r;
        var ele = $(lc);
        var m = r - 1;
        ele.find('#aname').attr('name', '[' + m + '].AssetName');
        ele.find('#tick').attr('name', '[' + m + '].AssetTicker');
        ele.find('#commi').attr('name', '[' + m + '].Commissions');
        ele.find('#ii').attr('name', '[' + m + '].InitialInvestment');
        ele.find('#sharep').attr('name', '[' + m + '].SharePrice');
        ele.find('#datep').attr('name', '[' + m + '].DatePurchased');
        ele.find('#quant').attr('name', '[' + m + '].Quantity');
        ele.attr('class', '' + m);
    }
    numb--;
}

function openTab(evt, cityName) {
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
    document.getElementById(cityName).style.display = "block";
    
}


