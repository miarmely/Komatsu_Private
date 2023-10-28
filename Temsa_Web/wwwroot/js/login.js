﻿import { updateResultLabel } from "./miarTools.js";

$(function () {
    const resultLabelId = "#p_resultLabel";
    const errorMessageColor = "red";
    const tokenKeyInSession = "token"
   
    $("form").submit((event) => {
        event.preventDefault();

        //#region reset resultLabel
        var resultLabel = $(resultLabelId);
        resultLabel.empty();
        //#endregion

        $.ajax({
            method: "POST",
            url: baseApiUrl + `/user/login?language=${language}`,
            contentType: "application/json",
            data: JSON.stringify({
                "TelNo": $("#inpt_telNo").val().trim(),
                "Password": $("#inpt_password").val().trim()
            }),
            dataType: "json",
            success: (response) => {
                $("form")[0].reset();  // reset inputs

                //#region save token to localStorage
                let token = response["token"];
                localStorage.setItem(tokenKeyInSession, token);
                //#endregion

                //#region call afterLogin action
                location.replace("/authentication/afterLogin" +
                    `?token=${token}` +
                    `&language=${language}`);
                //#endregion
            },
            error: (response) => {
                //#region write error to resultLabel
                updateResultLabel(
                    resultLabelId,
                    JSON.parse(response.responseText).errorMessage,
                    errorMessageColor,
                    "50px")
                //#endregion
            }
        });
    });
});