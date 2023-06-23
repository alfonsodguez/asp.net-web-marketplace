$('button[id^="btnMas"]').click(function (ev) {
    const ean = $(this).attr("id").split('-')[1]
    const valueLab = parseFloat($(this).siblings('label').text()) + 1

    $(this).siblings('button').first().prop('disabled', false)
    $(this).siblings('label').text(valueLab)
})