﻿//#region export variables
export let paginationInfosInJson;
export let entityCountOnTable;
//#endregion

//#region export functions

//#region file processes
export async function displayImageByNormalUrlAsync(
    folderPathAfterWwwroot,
    fileName,
    imgForAddUrl,
    inputForAddFileName,
    fileStatusLabel) {
    //#region before start
    await beforeDisplayImageAsync(
        imgForAddUrl,
        "#" + fileStatusLabel.attr("id"),
        inputForAddFileName);
    //#endregion

    //#region display image
    // add src to <img>
    imgForAddUrl.attr(
        "src",
        "/" + folderPathAfterWwwroot + "/" + fileName
    );

    // write file name to <input>
    inputForAddFileName.val(fileName);

    // reset file status label
    fileStatusLabel.empty();
    //#endregion
}

export async function displayImageByDataUrlAsync(
    selectedFileInfos,
    imgForAddDataUrl,
    fileStatusLabel,
    inputForAddFileName = null,
    afterLoad = null) {
    //#region before start
    let fileStatusLabel_oldMessage = fileStatusLabel.text();
    let fileStatusLabel_oldStyle = fileStatusLabel.attr("style");

    await beforeDisplayImageAsync(
        imgForAddDataUrl,
        "#" + fileStatusLabel.attr("id"),
        inputForAddFileName);
    //#endregion

    //#region read file as dataUrl
    let fileReader = new FileReader();
    fileReader.readAsDataURL(selectedFileInfos);
    //#endregion

    //#region when reading completed
    fileReader.onloadend = function (event) {
        //#region when successfull
        if (fileReader.readyState == fileReader.DONE) {
            //#region add dataUrl to src of <img>
            let dataUrl = event.target.result
            imgForAddDataUrl.attr("src", dataUrl);
            //#endregion

            //#region write file name to <input>
            if (inputForAddFileName != null)
                inputForAddFileName.val(selectedFileInfos.name);
            //#endregion

            //#region call function after load
            if (afterLoad != null)
                afterLoad();
            //#endregion
        }
        //#endregion

        //#region reset "file loading..." message  
        fileStatusLabel.empty();
        fileStatusLabel.attr("style", fileStatusLabel_oldStyle);
        fileStatusLabel.append(fileStatusLabel_oldMessage);
        //#endregion
    }
    //#endregion
}

export async function isFileTypeInvalidAsync(
    selectedFileInfos,
    fileType,
    fileNameInput) {
    //#region when file type invalid
    if (!selectedFileInfos.type.startsWith(fileType)) {
        // reset file name <input>
        fileNameInput.val("");

        return true;
    }
    //#endregion

    return false;
}

async function beforeDisplayImageAsync(
    imgForAddUrl,
    fileStatusLabelId,
    inputForAddFileName = null) {
    // remove old image
    imgForAddUrl.removeAttr("src");

    // reset file name <input>
    if (inputForAddFileName != null)
        inputForAddFileName.val("");

    // write "file loading..." message
    updateResultLabel(
        fileStatusLabelId,
        partnerInformationMessagesByLanguages[language]["fileLoading"],
        fileStatusLabel_color
    );
}
//#endregion

//#region descriptions
export let descriptions_currentColor;
export let descriptions_language = language;

export async function setDescriptionsLanguageAsync(newLanguage) {
    descriptions_language = newLanguage;
}

export async function getDescriptionKeyForSessionAsync(descriptionBaseKeyForSession) {
    return descriptionBaseKeyForSession + '-' + descriptions_language;
}

export async function changeDescriptionsButtonColorAsync(descriptionsButton, color) {
    descriptionsButton.css("color", color);
    descriptions_currentColor = color;
}

export async function click_descriptionDropdownItemAsync(
    clickedElement,
    descriptionsTextarea,
    descriptionsButton) {
    //#region change descriptions <button> color as "unsaved_color"
    descriptionsTextarea.val("");

    await changeDescriptionsButtonColorAsync(
        descriptionsButton,
        descriptions_unsavedColor);
    //#endregion

    //#region change descriptions <button> name
    descriptions_language = clickedElement.prop("innerText");

    descriptionsButton.empty();
    descriptionsButton.append(
        `<b>${description_baseButtonNameByLanguages[language]} (${descriptions_language})</b>`);
    //#endregion

    //#region populate descriptions in session to <textarea>
    // get descriptions from session
    let descriptionsInSession = JSON.parse(sessionStorage
        .getItem(sessionKeys_descriptionsOnDisplayPage));

    // when any description is exists in session
    if (descriptionsInSession != null  // when any descriptions exists in session
        && descriptionsInSession[descriptions_language] != undefined)  // when descriptions in selected language 
        //#region add description in session to <textarea>
        descriptionsTextarea.val(
            descriptionsInSession[descriptions_language]);
        //#endregion
    //#endregion
}

export async function click_descriptionsButtonAsync(descriptionsTextArea, descriptionsButton) {
    //#region get descriptions in session 
    let descriptionsInSession = JSON.parse(sessionStorage
        .getItem(sessionKeys_descriptionsOnDisplayPage));
    //#endregion

    //#region save updated descriptions to session
    descriptionsInSession[descriptions_language] = descriptionsTextArea.val();

    sessionStorage.setItem(
        sessionKeys_descriptionsOnDisplayPage,
        JSON.stringify(descriptionsInSession));
    //#endregion

    //#region change description button color to "saved color"
    await changeDescriptionsButtonColorAsync(
        descriptionsButton,
        descriptions_savedColor);
    //#endregion
}

export async function change_descriptionsTextareaAsync(descriptionsButton) {
    //#region initialize descriptions current color if not initialized
    if (descriptions_currentColor == null)
        descriptions_currentColor = descriptions_unsavedColor;
    //#endregion

    //#region change descriptions <button> color as "unsaved color"
    if (descriptions_currentColor == descriptions_savedColor)
        await changeDescriptionsButtonColorAsync(
            descriptionsButton,
            descriptions_unsavedColor);
    //#endregion
}
//#endregion

export function getDateTimeInString(dateTime) {
    //#region set year
    let date = new Date(dateTime);
    let year = date.getFullYear();
    //#endregion

    //#region set month
    let month = date.getMonth() + 1;

    // add '0' to head
    let monthInString = month < 10 ?
        `0${month}`  // add 0
        : month.toString();  // don't add
    //#endregion

    //#region set day
    let day = date.getDate();

    // add '0' to head
    let dayInString = day < 10 ?
        `0${day}`  // add 0
        : day.toString(); // don't add
    //#endregion

    //#region set hours
    let hours = date.getHours() + 3;

    // add '0' to head
    let hoursInString = hours < 10 ?
        `0${hours}`  // add 0
        : hours.toString();  // don't add
    //#endregion

    //#region set minutes
    let minutes = date.getMinutes();

    // add '0' to head
    let minutesInString = minutes < 10 ?
        `0${minutes}`  // add 0
        : minutes.toString();  // don't add
    //#endregion

    return `${dayInString}.${monthInString}.${year} - ${hoursInString}:${minutesInString}`;
}

export function getHeaderFromLocalInJson(headerName) {
    return JSON.parse(
        localStorage.getItem(headerName));
}

export function getTokenInSession() {
    return sessionStorage.getItem("token");
}

export function updateResultLabel(
    resultLabelId,
    message,
    color,
    marginT = "0px") {
    //#region reset resultLabel
    let resultLabel = $(resultLabelId);
    resultLabel.empty();
    //#endregion

    //#region change style
    resultLabel.attr("style",
        `color:	${color}; 
		margin-top: ${marginT};
		text-align: center`);
    //#endregion

    //#region write error to resultLabel
    resultLabel.removeAttr("hidden");  // show resultLabel
    resultLabel.append(message);
    //#endregion
}

export function updateErrorRow(
    errorRowId,
    message,
    color,
    marginT = "30px") {
    //#region show <td> of <tr> of error
    let tr_row_error = $(errorRowId);
    let td_error = tr_row_error.children("td");

    td_error.removeAttr("hidden");
    //#endregion

    //#region change style
    td_error.attr("style",
        `color:	${color}; 
		margin-top: ${marginT};
		text-align: center`);
    //#endregion

    //#region write error to <td> of error row
    td_error.empty();
    td_error.append(message);
    //#endregion
}

export function resetErrorRow(rowId) {
    //#region hide and reset <td> of error
    var td_error = $(`#${rowId}_error`)
        .children("td")

    td_error.attr("hidden", "");
    td_error.empty();
    //#endregion
}

export async function populateTable(
    entityType,
    specialUrl,
    language,
    pageNumber,
    pageSize,
    tableBody,
    columnNamesToBeFill,
    updateButtonName,
    nameOfPaginationHeader,
    lbl_entityQuantity,
    ul_pagination,
    errorMessageColor,
    paginationButtonQuantity,
    entityQuantity_message,
    refreshPaginationButtons) {

    //#region set variables
    descriptions_language = language; // set page language as default 

    let url = `${baseApiUrl}/${specialUrl}` +
        `?language=${language}` +
        `&pageNumber=${pageNumber}` +
        `&pageSize=${pageSize}`
    //#endregion

    $.ajax({
        method: "GET",
        url: url,
        headers: { "Authorization": jwtToken },
        contentType: "application/json",
        dataType: "json",
        beforeSend: () => {
            //#region reset table if not empty
            if (tableBody.children("tr").length != 0)
                tableBody.empty();
            //#endregion
        },
        success: (response, status, xhr) => {
            addEntitiesToTableAsync(response, language, tableBody, entityType, columnNamesToBeFill, updateButtonName);

            //#region get pagination infos from headers
            paginationInfosInJson = JSON.parse(
                xhr.getResponseHeader(nameOfPaginationHeader));
            //#endregion

            //#region update entity count label
            if (response.length != 0) {  // if any machine exists
                entityCountOnTable = paginationInfosInJson.CurrentPageCount;

                updateResultLabel(
                    "#" + lbl_entityQuantity.attr("id"),
                    `<b>${entityCountOnTable}/${pageSize}</b> ${entityQuantity_message}`,
                    "#7A7A7A"
                )
            }
            //#endregion

            //#region add pagination buttons
            if (refreshPaginationButtons)
                addPaginationButtonsAsync(
                    paginationInfosInJson,
                    paginationButtonQuantity,
                    ul_pagination);
            //#endregion

            controlPaginationBackAndNextButtonsAsync(paginationInfosInJson);
        },
        error: (response) => {
            //#region write error to resultLabel
            updateResultLabel(
                lbl_entityQuantity,
                JSON.parse(response.responseText).errorMessage,
                errorMessageColor);
            //#endregion
        },
    });
}

export async function setDisabledOfOtherUpdateButtonsAsync(rowId, pageSize, doDisabled) {
    //#region disable/enable other update buttons
    for (var rowNo = 1; rowNo <= pageSize; rowNo += 1) {
        let rowIdInLoop = `#tr_row${rowNo}`;

        //#region disable/enable update button
        if (rowIdInLoop != rowId) {
            // get update button
            let btn_update = $(rowIdInLoop)
                .children("#td_processes")
                .children("button")  // update button

            // when disabled wanted
            if (doDisabled)
                btn_update.attr("disabled", "");

            // when enabled wanted
            else
                btn_update.removeAttr("disabled");
        }
        //#endregion
    }
    //#endregion

}

export async function populateElementByAjaxOrLocalAsync(
    dataNameInLocal,
    apiUrl,
    func_populate,
    func_afterPopulated = null) {
    //#region get data from local
    let dataInLocal = JSON.parse(
        localStorage.getItem(dataNameInLocal));
    //#endregion

    //#region get data by ajax if not exists in local (ajax)
    if (dataInLocal == null  // data of any language not exists in local
        || dataInLocal[language] == null)  // data belong to language not exists in local
        $.ajax({
            method: "GET",
            url: baseApiUrl + apiUrl,
            headers: {
                "Authorization": jwtToken
            },
            contentType: "application/json",
            dataType: "json",
            success: (response) => {
                func_populate(response);

                //#region save data to local

                //#region initialize "dataInLocal"
                if (dataInLocal == null)  // when any data not exists
                    dataInLocal = {};

                dataInLocal[language] = response;
                //#endregion

                //#region add to local
                localStorage.setItem(
                    dataNameInLocal,
                    JSON.stringify(dataInLocal));
                //#endregion

                //#endregion

                //#region call function after populate process
                if (func_afterPopulated != null)
                    func_afterPopulated();
                //#endregion
            }
        });
    //#endregion

    //#region when data already in local
    else {
        func_populate(dataInLocal[language]);

        //#region call function after populate process
        if (func_afterPopulated != null)
            func_afterPopulated();
        //#endregion
    }
    //#endregion
}

export async function populateSelectAsync(select, options, optionToBeDisplay = null) {
    //#region add <option>'s to <select>
    for (let index in options) {
        let option = options[index];

        select.append(
            `<option>${option}</option>`
        )
    }
    //#endregion

    //#region set option to be display on <select>
    if (optionToBeDisplay != null)
        select.val(optionToBeDisplay);
    //#endregion
}

export async function setDisabledOfButtonAsync(doDisabled, button, bgColor) {
    //#region disable the button
    if (doDisabled) {
        button.attr("disabled", "");
        button.css("background-color", bgColor);
    }
    //#endregion

    //#region active the button
    else {
        button.removeAttr("disabled");
        button.css("background-color", bgColor);
    }
    //#endregion
}

export async function setDisabledOfButtonsAsync(doDisabled, buttonIds, bgColor) {
    //#region disable/enable multiple button
    for (let index in buttonIds) {
        //#region disable the button
        let button = $(buttonIds[index]);

        if (doDisabled) {
            button.attr("disabled", "");
            button.css("background-color", bgColor);
        }
        //#endregion

        //#region active the button
        else {
            button.removeAttr("disabled");
            button.css("background-color", bgColor);
        }
        //#endregion
    }
    //#endregion
}

export async function getErrorMessageForMachineAsync(responseText) {
    //#region set variables
    const propertyNamesGuideByLanguages = {
        "TR": {
            "ImageName": "Resmin Adı",
            "MainCategoryName": "Ana Kategori",
            "SubCategoryName": "Alt Kategori",
            "Model": "Model",
            "BrandName": "Marka",
            "Year": "Yıl",
            "Stock": "Stok Adedi",
            "HandStatus": "El Durumu",
            "PdfName": "Pdf'in Adı",
            "DescriptionInTR": "TR Açıklama",
            "DescriptionInEN": "EN Açıklama"
        },
        "EN": {
            "ImageName": "name of Image",
            "MainCategoryName": "Maincategory",
            "SubCategoryName": "Subcategory",
            "Model": "Model",
            "BrandName": "Brand",
            "Year": "Year",
            "Stock": "Stock",
            "HandStatus": "Hand Status",
            "PdfName": "Name of Pdf",
            "DescriptionInTR": "TR Description",
            "DescriptionInEN": "EN Description"
        }
    };
    let errorDetails = JSON.parse(responseText);
    let errorMessage;
    //#endregion

    //#region set error message

    //#region when error type is "Format Error"
    if (formatErrorCodes.findIndex(e =>
        e == errorDetails["errorCode"]) != -1) {
        //#region get label name
        let infosInErrorMessage = JSON.parse(
            errorDetails["errorMessage"]);

        let labelName = propertyNamesGuideByLanguages
        [language]
        [infosInErrorMessage.PropertyName];
        //#endregion

        //#region add error message
        switch (errorDetails["errorCode"]) {
            case "FE-MinL":
                //#region set minumum length error message
                errorMessage = language == "TR" ?
                    `"${labelName}" en az ${infosInErrorMessage.MinLength} karakter uzunluğunda olmalı`  // TR
                    : `minumum chars length of "${labelName}" must be ${infosInErrorMessage.MinLength}`  // EN
                //#endregion
                break;
            case "FE-MaxL":
                //#region set max length error message
                errorMessage = language == "TR" ?
                    `"${labelName}" en fazla ${infosInErrorMessage.MaxLength} karakter uzunluğunda olmalı`  // TR
                    : `maximum chars length of "${labelName}" must be ${infosInErrorMessage.MaxLength}`  // EN
                //#endregion
                break;
            case "FE-MinV":
                //#region set minumum value error message
                errorMessage = language == "TR" ?
                    `"${labelName}" en az ${infosInErrorMessage.MinValue} değerinde olmalı`  // TR
                    : `minumum value of "${labelName}" must be ${infosInErrorMessage.MinValue}`  // EN
                //#endregion
                break;
            case "FE-MaxV":
                //#region set max value error message
                errorMessage = language == "TR" ?
                    `"${labelName}" en fazla ${infosInErrorMessage.MaxValue} değerinde olmalı`  // TR
                    : `maximum value of "${labelName}" must be ${infosInErrorMessage.MaxValue}`  // EN
                //#endregion
                break;
        }
        //#endregion
    }
    //#endregion

    //#region for other error types
    else
        errorMessage = errorDetails.errorMessage;
    //#endregion

    //#endregion

    return errorMessage;
}

export async function addPaginationButtonsAsync(
    paginationInfosInJson,
    paginationButtonQuantity,
    ul_pagination) {
    //#region set buttonQauntity for pagination
    let buttonQuantity =
        paginationInfosInJson.TotalPage < paginationButtonQuantity ?
            paginationInfosInJson.TotalPage
            : paginationButtonQuantity
    //#endregion

    //#region reset paginationButtons if exists
    if (ul_pagination.children("li").length != 0)
        ul_pagination.empty()
    //#endregion

    //#region add paginationBack button
    ul_pagination.append(
        `<li>
		<a id="a_paginationBack" href="#" hidden>
			<i class="fa fa-chevron-left"></i>
		</a>
	</li>`);
    //#endregion

    //#region add pagination buttons
    for (let pageNo = 1; pageNo <= buttonQuantity; pageNo += 1)
        ul_pagination.append(
            `<li>
			<a href="#"> 
				${pageNo}
			</a>
		</li> `
        );
    //#endregion

    //#region add paginationNext button
    ul_pagination.append(
        `<li>
		<a id="a_paginationNext" href="#" hidden>
			<i class="fa fa-chevron-right"></i>
		</a>
	</li>`);
    //#endregion
}

export async function controlPaginationBackAndNextButtonsAsync(paginationInfosInJson) {
    // when total page count more than 1
    if (paginationInfosInJson.TotalPage > 1) {
        //#region for paginationBack button
        // hide
        if (paginationInfosInJson.CurrentPageNo == 1)
            $("#a_paginationBack").attr("hidden", "");

        // show
        else
            $("#a_paginationBack").removeAttr("hidden");
        //#endregion

        //#region for paginationNext button
        // hide
        if (paginationInfosInJson.CurrentPageNo == paginationInfosInJson.TotalPage)
            $("#a_paginationNext").attr("hidden", "");

        // show
        else
            $("#a_paginationNext").removeAttr("hidden");
        //#endregion
    }
}

async function addEntitiesToTableAsync(
    response,
    language,
    tableBody,
    entityType,
    columnNamesToBeFill,
    updateButtonName) {
    await new Promise(resolve => {
        //#region set variables
        let columnQuantityOnTable = columnNamesToBeFill.length + 3;  // 3: 1-> checkBox column, 2-> processes column, 3-> blank column
        let rowNo = 1;
        let rowId;
        let row;
        //#endregion

        //#region add entities to table
        response.forEach(entityView => {
            //#region add checkbox to row
            rowId = `tr_row${rowNo}`

            //#region when entity type is machine
            if (entityType == "machine")
                tableBody.append(
                    `<tr id= "${rowId}" class= ${entityView.id}>
                        <td id="td_checkBox">
					        <label class="i-checks m-b-none">
						        <input type="checkbox"><i></i>
					        </label>
				        </td>
                    </tr>`);
            //#endregion

            //#region when entity type is user
            else
                tableBody.append(
                    `<tr id= "${rowId}">
                        <td id="td_checkBox">
					        <label class="i-checks m-b-none">
						        <input type="checkbox"><i></i>
					        </label>
				        </td>
                    </tr>`);

            row = $("#" + rowId);
            //#endregion

            //#endregion

            //#region add column values of entity as dynamic
            for (let index in columnNamesToBeFill) {
                let columnName = columnNamesToBeFill[index];

                //#region set columnValue
                let columnValue = columnName != "descriptions" ?
                    entityView[columnName]
                    : entityView[columnName][language]
                //#endregion

                //#region when column name is not "createdAt"
                if (columnName != "createdAt")
                    row.append(
                        `<td id="td_${columnName}">${columnValue}</td>`
                    );
                //#endregion

                //#region when column name is "createdAt"
                else
                    row.append(
                        `<td id="td_${columnName}">${getDateTimeInString(columnValue)}</td>`
                    );
                //#endregion
            }
            //#endregion

            //#region add update button to row
            row.append(
                `<td id="td_processes">
				    <button id="btn_update" ui-toggle-class="">
					    <i class="fa fa-pencil text-info">
						    ${updateButtonName}
					    </i>
				    </button>
			    </td>
                <td style="width:30px;"></td>`
            );
            //#endregion

            //#region add error row to row
            tableBody.append(
                `<tr hidden></tr>
			    <tr id="tr_row${rowNo}_error">
		            <td id="td_error" colspan=${columnQuantityOnTable} hidden>
                    </td>
			    </tr>`
            );
            //#endregion

            //#region add descriptions of machines to session if entity is machine
            if (entityType == "machine")
                sessionStorage.setItem(
                    rowId,
                    JSON.stringify({
                        "descriptions": entityView.descriptions
                    }));
            //#endregion

            rowNo += 1;
        });
        //#endregion

        resolve();
    })
}
//#endregion