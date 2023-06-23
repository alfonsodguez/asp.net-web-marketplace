$('button[id^="btnMenos"]').click(function (ev) {
    const ean = $(this).attr("id").split('-')[1]
    const lab = $(this).siblings('label')
    const valueLab = lab.text()

    if (valueLab > 1) {
        $(lab).text(parseInt(valueLab) - 1)
    } else {
        $(lab).siblings('button').first().prop('disabled', true)
    }
})