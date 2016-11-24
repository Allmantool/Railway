'use strict';

//namespace
var appNomenclature = window.appNomenclature || {};

//constuctor
appNomenclature.Scroll = function (keyKrt, confirmed, nkrt, errorState, signAdjustmentlist, dtBuhOtchet, dtClaim, sumClaim, ndsClaim, startDateScr, endDateScr, scrCharges, recordCount, dateWorking, counterVersion) {
    this.keyScr = keyKrt;
    this.numScr = nkrt;
    this.confirmed = confirmed;
    this.errState = errorState;
    this.signAdjustmentList = signAdjustmentlist;
    this.DtBookkeeper = dtBuhOtchet;
    this.DtClaim = dtClaim;
    this.sumClaim = sumClaim;
    this.VatClaim = ndsClaim;
    this.startDateScr = startDateScr;
    this.endDateScr = endDateScr;
    this.scrCharges = scrCharges;
    this.recordCount = recordCount;
    this.dateWorking = dateWorking;
    this.counterVersion = counterVersion;
};