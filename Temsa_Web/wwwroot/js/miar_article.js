﻿//#region variables
export let buffers = {
    "div_articles": "",
    "path_articleVideos": "",
}
export const style_article = {
    "width": 370,
    "height": 580,
    "marginT": 10,
    "marginB": 10,
    "marginR": 20,
    "marginL": 20,
    "paddingT": 10,
    "paddingB": 10,
    "paddingR": 10,
    "paddingL": 10,
    "border": 6,
};
export const div_article_video_id = "div_article_video";
export const div_article_image_id = "div_articl_image";
export const div_article_info_id = "div_article_info";
export const div_article_button_id = "div_article_button";
export const art_baseId = "art_";
export const path_playImage = "images/play.png";
export const btn_pdf_id = "btn_pdf";
export const headerOfPageHeight = 80;
let articleInfos_lastUploadedPlayImage = {};
let articleInfos_lastUploadedVideo = {
    "article": null
};

//#region article elements styles

//#region when article type is "video and text" (VT)
export const style_a_pdfButton_fontSize = 18;
export const style_a_pdfButton_paddingTB = 6;
export const style_a_pdfButton_paddingRL = 40;

export const style_div_video_marginB_VT = 20;
export const style_div_video_width_VT = style_article.width - (style_article.border * 2) - style_article.paddingR - style_article.paddingL
export const style_div_video_height_VT = (style_article.height - (style_article.border * 2) - style_article.paddingT - style_article.paddingB - style_div_video_marginB_VT) / 2;

export const style_div_button_width_VT = style_div_video_width_VT;
export const style_div_button_height_VT = (style_a_pdfButton_fontSize + 9) + (style_a_pdfButton_paddingTB * 2);

export const style_div_info_width_VT = style_div_video_width_VT;
export const style_div_info_height_VT = style_div_video_height_VT - style_div_button_height_VT;

export const style_vid_width_VT = style_div_video_width_VT;
export const style_vid_height_VT = style_div_video_height_VT;

export const style_img_play_width_VT = style_vid_width_VT / 2.5;
export const style_img_play_height_VT = style_vid_height_VT / 2.2;
export const style_img_play_marginT_VT = (style_vid_height_VT - style_img_play_height_VT) / 2;
export const style_img_play_marginB_VT = style_img_play_marginT_VT;
export const style_img_play_marginR_VT = (style_vid_width_VT - style_img_play_width_VT) / 2;
export const style_img_play_marginL_VT = style_img_play_marginR_VT;
//#endregion

//#region when article type is "image and text" (IT)
export const style_div_img_width_IT = style_div_video_width_VT;
export const style_div_img_height_IT = style_div_video_height_VT;
export const style_div_img_marginB_IT = style_div_video_marginB_VT;

export const style_div_info_width_IT = style_div_info_width_VT;
export const style_div_info_height_IT = style_div_info_height_VT;

export const style_img_width_IT = style_div_video_width_VT;
export const style_img_height_IT = style_div_video_height_VT;
//#endregion

//#region when article type is only "text" (T)
export const style_div_info_width_T = style_div_info_width_VT;
export const style_div_info_height_T = style_div_info_height_VT;
//#endregion

//#endregion

//#endregion

//#region events
export async function click_playImageAsync(article) {
    removeVideoOfPreviousArticle();
    hidePlayImage(article);

    //#region load article video
    let videoName = article_idsAndMachineInfos[article.attr("id")]["videoName"];

    article
        .find("video")
        .attr({
            "src": "/" + buffers.path_articleVideos + "/" + videoName,
            "controls": "",
            "autoplay": ""
        });

    // save article of last uploaded video 
    articleInfos_lastUploadedVideo["article"] = article;
    //#endregion
}

export async function mouseover_articleVideoAsync(event, article) {
    //#region when video is exists on article
    if (isVideoExists(article))
        return;
    //#endregion

    //#region save article video infos

    //#region save video infos
    let minPageX = event.pageX - event.offsetX;
    let minPageY = event.pageY - event.offsetY;

    // save infos
    articleInfos_lastUploadedPlayImage = {
        "article": article,
        "minPageX": minPageX,
        "maxPageX": minPageX + style_vid_width_VT,
        "minPageY": minPageY,
        "maxPageY": minPageY + style_vid_height_VT
    };
    //#endregion

    //#endregion

    showPlayImage(article);
}

export async function mouseout_articleVideoDivAsync(event, article) {
    //#region when video is exists on article
    if (isVideoExists(article))
        return;
    //#endregion

    //#region when mouse isn't over on header AND is over article video 
    if (event.clientY > headerOfPageHeight) {
        //#region when mouse is over article video (return)
        let currentMouseX = event.pageX;
        let currentMouseY = event.pageY;

        if (currentMouseX > articleInfos_lastUploadedPlayImage["minPageX"]
            && currentMouseX < articleInfos_lastUploadedPlayImage["maxPageX"]
            && currentMouseY > articleInfos_lastUploadedPlayImage["minPageY"]
            && currentMouseY < articleInfos_lastUploadedPlayImage["maxPageY"]) {
            return;
        }
        //#endregion
    }
    //#endregion

    //#region when mouse is over on header OR is out from article video
    hidePlayImage(article);
    //#endregion
}

export async function ended_articleVideoAsync() {
    removeArticleVideo(articleInfos_lastUploadedVideo["article"]);
}
//#endregion

//#region functions
export function setVariablesForArticle(variables) {
    //#region initialize variables
    for (let variableName in variables)
        // when variable name is exists in "buffers"
        if (buffers[variableName] != undefined)
            buffers[variableName] = variables[variableName];
    //#endregion
}

export function showPlayImage(article) {
    //#region hide video
    article.find("video")
        .attr("hidden", "");
    //#endregion

    //#region load play image
    let img_play = article.find("img");
    img_play.attr({
        "src": "/" + path_playImage,
        "alt": "play"
    });
    //#endregion

    //#region add play image styles
    img_play.css({
        "width": style_img_play_width_VT,
        "height": style_img_play_height_VT,
        "margin-top": style_img_play_marginT_VT,
        "margin-bottom": style_img_play_marginB_VT,
        "margin-right": style_img_play_marginR_VT,
        "margin-left": style_img_play_marginL_VT
    });
    //#endregion

    img_play.removeAttr("hidden"); // show play <img>
}

export function hidePlayImage(article) {
    // remove attributes of play image
    let img_play = article.find("img");
    img_play.removeAttr("src alt style");

    // hide play image
    img_play.attr("hidden", "");

    // show video poster
    article
        .find("video")
        .removeAttr("hidden");
}

export function isVideoExists(article) {
    let src = article
        .find("video")
        .attr("src");

    return src != undefined;
}

export function removeArticleVideo(article) {
    //#region remove attributes article video
    let video = article
        .find("video");

    video.removeAttr("src controls autoplay");
    //#endregion

    video.load();
}

export function removeVideoOfPreviousArticle() {
    //#region when video isn't uploaded to any article previously
    let previousArticle = articleInfos_lastUploadedVideo["article"];

    if (previousArticle == undefined)
        return;
    //#endregion

    //#region when video is exists on previous article
    if (isVideoExists(previousArticle)) {
        removeArticleVideo(previousArticle);
    }
    //#endregion
}

export async function removeLastUploadedArticleVideoAsync() {
    //#region remove last uploaded article video if exists
    let article = articleInfos_lastUploadedVideo["article"];

    if (article != null  // when at least one video has been opened previously
        && isVideoExists(article))
        removeArticleVideo(article);
    //#endregion
}

export async function addArticlesAsync(articleType) {  // articleType: "imageAndText", "videoAndText", "text"
    //#region add articles
    for (let index = 0; index < article_totalCount; index++) {
        //#region add articles by article type
        let articleId = art_baseId + index;
        let article;

        switch (articleType) {
            case "videoAndText":
                //#region add article with video and text
                if (articleType == "videoAndText")
                    buffers.div_articles.append(`
                        <article id="${articleId}"  class="article" style="text-align: center">
                            <div id="${div_article_video_id}">
                                <img class="img_play"  hidden/>
                                <video poster="">
                                    <source src="" type=""></source>
                                </video>
                            </div>

                            <div id="${div_article_info_id}">
                            </div>

                            <div id="${div_article_button_id}">
                                <ul>
                                    <li class="btn btn_article">
                                        <a target="blank">PDF</a>
                                    </li>
                                </ul>  
                            </div>
                        </article>`
                    );
                //#endregion

                //#region add styles of article elements
                // <video> styles
                article = $("#" + articleId);
                article
                    .find("video")
                    .css({
                        "width": style_vid_width_VT,
                        "height": style_vid_height_VT
                    });

                // video <div> styles
                article
                    .find('#' + div_article_video_id)
                    .css({
                        "width": style_div_video_width_VT,
                        "height": style_div_video_height_VT,
                        "margin-bottom": style_div_video_marginB_VT
                    });

                // info <div> styles
                article
                    .find('#' + div_article_info_id)
                    .css({
                        "width": style_div_info_width_VT,
                        "height": style_div_info_height_VT
                    });

                // button <div> styles
                article
                    .find("#" + div_article_button_id)
                    .css({
                        "width": style_div_button_width_VT,
                        "height": style_div_button_height_VT,
                    })

                // pdf button <li> styles
                article
                    .find("#" + div_article_button_id + " li")
                    .css({
                        "padding-top": style_a_pdfButton_paddingTB,
                        "padding-bottom": style_a_pdfButton_paddingTB,
                        "padding-right": 0,
                        "padding-left": 0
                    });

                // pdf button <a> styles
                article
                    .find("#" + div_article_button_id + " a")
                    .css({
                        "padding-top": style_a_pdfButton_paddingTB,
                        "padding-bottom": style_a_pdfButton_paddingTB,
                        "padding-right": style_a_pdfButton_paddingRL,
                        "padding-left": style_a_pdfButton_paddingRL,
                        "font-size": style_a_pdfButton_fontSize
                    });
                //#endregion

                break;
            case "imageAndText":
                //#region add article with image and text
                buffers.div_articles.append(`
                    <article id="${articleId}"  class="article">
                        <div id="${div_article_image_id}" >
                            <img src=""  alt=""  title="" />
                        </div>

                        <div id="${div_article_info_id}" >
                        </div>
                    </article>`
                );
                //#endregion

                //#region add styles of article elements
                // <img> styles
                article = $('#' + articleId);
                article
                    .find("img")
                    .css({
                        "width": style_img_width_IT,
                        "height": style_img_height_IT
                    });

                // image <div> styles
                article
                    .find('#' + div_article_image_id)
                    .css({
                        "width": style_div_img_width_IT,
                        "height": style_div_img_height_IT,
                        "margin-bottom": style_div_img_marginB_IT
                    });

                // info <div> styles
                article
                    .find('#' + div_article_info_id)
                    .css({
                        "width": style_div_info_width_IT,
                        "height": style_div_info_height_IT
                    });
                //#endregion

                break;
            case "text":
                //#region add article with only text
                buffers.div_articles.append(`
                    <article id="${articleId}"  class="article">
                        <div id="${div_article_info_id}" >
                        </div>
                    </article>`
                );
                //#endregion

                //#region add styles of article elements
                // info <div> styles
                article = $('#' + articleId);
                article
                    .find('#' + div_article_info_id)
                    .css({
                        "width": style_div_info_width_T,
                        "height": style_div_info_height_T
                    });
                //#endregion

                break;
        }
        //#endregion

        //#region add <article> style
        article.css({
            "width": style_article.width,
            "height": style_article.height,
            "margin-top": style_article.marginT,
            "margin-bottom": style_article.marginB,
            "margin-right": style_article.marginR,
            "margin-left": style_article.marginL,
            "padding-top": style_article.paddingT,
            "padding-bottom": style_article.paddingB,
            "padding-right": style_article.paddingR,
            "padding-left": style_article.paddingL,
            "border-width": style_article.border
        });
        //#endregion
    }
    //#endregion

    await alignArticlesToCenterAsync();
}

export async function alignArticlesToCenterAsync() {
    //#region set padding left and right of articles <div>
    let divClientWidth = buffers.div_articles.prop("clientWidth");
    let netArticleWidth = style_article.width + style_article.marginL + style_article.marginR;
    let articleCountOnOneRow = Math.floor(divClientWidth / netArticleWidth);
    let whiteSpaceWidth = divClientWidth - (netArticleWidth * articleCountOnOneRow);

    buffers.div_articles.css({
        "padding-left": Math.floor(whiteSpaceWidth / 2),
        "padding-right": Math.floor(whiteSpaceWidth / 2)
    });
    //#endregion

    //#region set height of articles <div>
    let netArticleHeight = style_article.height + style_article.marginT + style_article.marginB;
    let totalRowCount = article_totalCount % articleCountOnOneRow == 0 ?
        Math.floor(article_totalCount / articleCountOnOneRow)  // when article count of all rows is equal
        : Math.floor(article_totalCount / articleCountOnOneRow) + 1  // when article count of last row is different

    buffers.div_articles.css(
        "height",
        netArticleHeight * totalRowCount);
    //#endregion
}

export async function isSidebarOpenAsync() {
    //#region when sidebar is closed
    let closedSidebarClass = "nav-collapse hide-left-bar";

    if ($("#sidebar").attr("class") == closedSidebarClass)
        return false;
    //#endregion

    return true;
}
//#endregion