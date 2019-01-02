$(document).ready(function () {
    loadPlans();
    onProductChange();
    onPlanChange();
});

function loadPlans() {
    var url = '/Identity/Account/Manage/Subscriptions?handler=Products';
    var productId = $('.subscription-products').val();
    var data = { "productId": productId };
    var plans = $('.subscription-plans');
    plans.empty();
    $.getJSON(url, data, function (productPlans) {
        if (productPlans !== null) {
            $.each(productPlans, function (index, htmlData) {
                plans.append($('<option />', {
                    value: htmlData.value,
                    text: htmlData.text
                }));
            });
            loadPlanPrice();
        }
    });
}
function loadPlanPrice() {
    var url = '/Identity/Account/Manage/Subscriptions?handler=PlanPrice';
    var planId = $('.subscription-plans').val();
    var data = { "planId": planId };
    var planControl = $('.plan-price');
    var stripeButton = $('.stripe-button');

    planControl.empty();
    $.getJSON(url, data, function (planPrice) {
        if (planPrice !== null) {
            var currency = planPrice / 100;
            planControl.append($('<span />', {
                text: '$' + currency + '.00'
            }));

            stripeButton.attr('data-amount', planPrice);
        }
    });
}

function onProductChange() {
    $('.subscription-products').change(function () {
        loadPlans();
    });
}

function onPlanChange() {
    $('.subscription-plans').change(function () {
        loadPlanPrice();
    });
}

