﻿import { updateResultLabel } from "./miar_tools.js";
import { clicked_languageDropdown, populateLanguageDropdown, updateDefaultFlagAndLanguage } from "./miar_header.js";

$(function () {
    //#region variables
    const resultLabelId = "#p_resultLabel";
    const errorMessageColor = "red";
    const inputPlaceHolders = inputPlaceHoldersByLanguages[language];
    //#endregion

    //#region events
    $("form").submit((event) => {
        event.preventDefault();

        //#region reset resultLabel
        var resultLabel = $(resultLabelId);
        resultLabel.empty();
        //#endregion

        $.ajax({
            method: "POST",
            url: baseApiUrl + `/user/login/web?language=${language}`,
            contentType: "application/json",
            data: JSON.stringify({
                "TelNo": $("#inpt_telNo").val().trim(),
                "Password": $("#inpt_password").val().trim()
            }),
            dataType: "json",
            success: (response) => {
                $("form")[0].reset();  // reset inputs

                //#region add token and language to local
                let token = response["token"];
                localStorage.setItem("token", token);
                localStorage.setItem("language", language);
                //#endregion

                //#region call afterLogin action
                location.replace("/authentication/afterLogin" +
                    `?token=${token}`);
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
    $("#" + ul_languages_id).click(() =>
        clicked_languageDropdown($(":focus"))
    )
    //#endregion

    //#region functions
    async function populateHtmlAsync() {
        await new Promise(resolve => {
            //#region populate <form>
            $("#h2_mainTitle").append(mainTitleByLanguages[language])
            $("#inpt_telNo").attr("placeholder", inputPlaceHolders.telNo)
            $("#inpt_password").attr("placeholder", inputPlaceHolders.password)
            $("#a_iForgotMyPassword").append(iForgotMyPasswordByLanguages[language])
            $("#btn_login").attr("value", loginButtonNameByLanguages[language]);
            //#endregion

            //#region set default flag and language on language dropdown
            updateDefaultFlagAndLanguage(
                img_displayingFlag_id,
                spn_displayingLanguage_id);
            //#endregion

            //#region add languages to dropdown (static)
            populateLanguageDropdown(
                ul_languages_id,
                ["TR", "EN"]  
            )
            //#endregion

            resolve();
        });
    }
    //#endregion

    populateHtmlAsync();
});