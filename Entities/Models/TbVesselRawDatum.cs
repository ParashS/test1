using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Entities.Models
{
    [Table("tb_VesselRawData")]
    public partial class TbVesselRawDatum
    {
        public TbVesselRawDatum()
        {
            TbVesselBunkerStemsAndSurveysRawData = new HashSet<TbVesselBunkerStemsAndSurveysRawDatum>();
            TbVesselBunkerTankRawData = new HashSet<TbVesselBunkerTankRawDatum>();
            TbVesselForobRawData = new HashSet<TbVesselForobRawDatum>();
            TbVesselUpcomingPortRawData = new HashSet<TbVesselUpcomingPortRawDatum>();
            TbVesselsAuxilliaryRawData = new HashSet<TbVesselsAuxilliaryRawDatum>();
            TbCargoHandlingData = new HashSet<TbCargoHandlingDatum>();
            TbPortActivitiesData = new HashSet<TbPortActivities>();
        }

        [Key]
        public long Id { get; set; }
        public long? ParentVesselRawDataId { get; set; }
        [Column(TypeName = "geography")]
        public Point? GeographyLocation { get; set; }
        [StringLength(1024)]
        [Unicode(false)]
        public string? ChiefEngineerName { get; set; }
        [StringLength(1024)]
        [Unicode(false)]
        public string? MasterName { get; set; }
        [StringLength(512)]
        [Unicode(false)]
        public string? SourceFeed { get; set; }
        public string? AttachmentId { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? ReportType { get; set; }
        public int VersionNumber { get; set; }
        [StringLength(1024)]
        public string? FormIdentifier { get; set; }
        [StringLength(1024)]
        public string? CompanyCode { get; set; }
        [StringLength(1024)]
        public string? CompanyName { get; set; }
        [StringLength(1024)]
        public string? VesselCode { get; set; }
        public DateTime? SubmittedDate { get; set; }
        [StringLength(1024)]
        public string? Status { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? LatestResubmissionDate { get; set; }
        [Column("FormGUID")]
        [StringLength(1024)]
        public string? FormGuid { get; set; }
        [StringLength(1024)]
        public string? ImoNumber { get; set; }
        [StringLength(1024)]
        public string? UnCode { get; set; }
        public string? VesselName { get; set; }
        public int? VoyageNo { get; set; }
        public string? VesselCondition { get; set; }
        [Column("FWDDraft", TypeName = "decimal(27, 20)")]
        public decimal? Fwddraft { get; set; }
        [Column("AFTDraft", TypeName = "decimal(27, 20)")]
        public decimal? Aftdraft { get; set; }
        [Column("Arrival_Position")]
        [StringLength(1024)]
        public string? ArrivalPosition { get; set; }
        [StringLength(1024)]
        public string? Port { get; set; }
        [StringLength(1024)]
        public string? Latitude { get; set; }
        [StringLength(1024)]
        public string? Longitude { get; set; }
        public DateTime? ReportTime { get; set; }
        [StringLength(50)]
        public string? ReportTimeString { get; set; }
        [Column("Time_Drop_of_Anchor")]
        [StringLength(50)]
        public string? TimeDropOfAnchor { get; set; }
        [Column(TypeName = "decimal(27, 20)")]
        public decimal? SteamingHrs { get; set; }
        [Column(TypeName = "decimal(27, 20)")]
        public decimal? MainEngineHrs { get; set; }
        [Column(TypeName = "decimal(27, 20)")]
        public decimal? ObservedDistance { get; set; }
        [Column(TypeName = "decimal(27, 20)")]
        public decimal? EngineDistance { get; set; }
        [Column("Logged_Distance", TypeName = "decimal(27, 20)")]
        public decimal? LoggedDistance { get; set; }
        [Column(TypeName = "decimal(27, 20)")]
        public decimal? ObsSpeed { get; set; }
        [Column("CPSpeed", TypeName = "decimal(27, 20)")]
        public decimal? Cpspeed { get; set; }
        [Column("Logged_Speed", TypeName = "decimal(27, 20)")]
        public decimal? LoggedSpeed { get; set; }
        [Column("RPM", TypeName = "decimal(27, 20)")]
        public decimal? Rpm { get; set; }
        [Column("MEOutputPct", TypeName = "decimal(27, 20)")]
        public decimal? MeoutputPct { get; set; }
        [Column(TypeName = "decimal(27, 20)")]
        public decimal? Slip { get; set; }
        [Column(TypeName = "decimal(27, 20)")]
        public decimal? WindForce { get; set; }
        [StringLength(1024)]
        public string? WindDirection { get; set; }
        [Column("Forob_AsOfDate")]
        [StringLength(50)]
        public string? ForobAsOfDate { get; set; }
        [Column("Fresh_Water_Produced_since_last_report_Mts", TypeName = "decimal(27, 20)")]
        public decimal? FreshWaterProducedSinceLastReportMts { get; set; }
        [Column("FreshWaterROB", TypeName = "decimal(27, 20)")]
        public decimal? FreshWaterRob { get; set; }
        [Column("Fresh_Water_for_Tank_Cleaning", TypeName = "decimal(27, 20)")]
        public decimal? FreshWaterForTankCleaning { get; set; }
        [Column("ETD")]
        public string? Etd { get; set; }
        [Column("NOR_Tendered_Time")]
        public string? NorTenderedTime { get; set; }
        [Column("LOP_issued")]
        public string? LopIssued { get; set; }
        [Column("Free_Pratique_Granted")]
        public string? FreePratiqueGranted { get; set; }
        [Column("ETB")]
        [StringLength(50)]
        public string? Etb { get; set; }
        public string? Stoppages { get; set; }
        [Column("Instructed_RPM")]
        public string? InstructedRpm { get; set; }
        public string? Remarks { get; set; }
        [Column("Distance__Vessel_Explanation")]
        public string? DistanceVesselExplanation { get; set; }
        [Column("Auxiliary_Engine_Instructions")]
        public string? AuxiliaryEngineInstructions { get; set; }
        [Column("Reason_to_omit_report_from_Performance")]
        public string? ReasonToOmitReportFromPerformance { get; set; }
        [Column("SlopsROB")]
        public string? SlopsRob { get; set; }
        [Column("Slops__Annex_1_or_Annex_2")]
        public string? SlopsAnnex1OrAnnex2 { get; set; }
        [Column("Grade_of_Slops")]
        public string? GradeOfSlops { get; set; }
        [Column("Slop_Stowage")]
        public string? SlopStowage { get; set; }
        [Column("Bunker_Tank_Instructions")]
        public string? BunkerTankInstructions { get; set; }
        [Column("Bunker_Rob_Instructions")]
        public string? BunkerRobInstructions { get; set; }
        [Column("Reason_for_Other_Consumption1")]
        public string? ReasonForOtherConsumption1 { get; set; }
        [Column("__PSId")]
        public string? Psid { get; set; }
        public string? Location { get; set; }
        public string? Event { get; set; }
        [Column(TypeName = "decimal(27, 20)")]
        public decimal? DistanceToGo { get; set; }
        [Column("SWBWFW")]
        public string? Swbwfw { get; set; }
        [Column("Est_Fwd_Draft", TypeName = "decimal(27, 20)")]
        public decimal? EstFwdDraft { get; set; }
        [Column("Est_Aft_Draft", TypeName = "decimal(27, 20)")]
        public decimal? EstAftDraft { get; set; }
        [Column("SWBWFW_Est")]
        public string? SwbwfwEst { get; set; }
        [Column("Total_Cargo_on_Board", TypeName = "decimal(27, 20)")]
        public decimal? TotalCargoOnBoard { get; set; }
        [Column("Cargo_Grade")]
        public string? CargoGrade { get; set; }
        [Column(TypeName = "decimal(27, 20)")]
        public decimal? FreshWaterMade { get; set; }
        [Column("Bunker_Stem_and_Survey_Description")]
        public string? BunkerStemAndSurveyDescription { get; set; }
        [Column("Inspections_In_Port_1")]
        public string? InspectionsInPort1 { get; set; }
        [Column("Inspections_In_Port_2")]
        public string? InspectionsInPort2 { get; set; }
        [Column("Inspections_In_Port_3")]
        public string? InspectionsInPort3 { get; set; }
        [Column("Inspect_Co_1")]
        public string? InspectCo1 { get; set; }
        [Column("Inspect_Co_2")]
        public string? InspectCo2 { get; set; }
        [Column("Inspect_Co_3")]
        public string? InspectCo3 { get; set; }
        [Column("Date_1")]
        public string? Date1 { get; set; }
        [Column("Date_2")]
        public string? Date2 { get; set; }
        [Column("Date_3")]
        public string? Date3 { get; set; }
        [Column("Number_of_Tugs_In")]
        public string? NumberOfTugsIn { get; set; }
        [Column("Tug_In_Start_Time")]
        public string? TugInStartTime { get; set; }
        [Column("Number_of_Tugs_Out")]
        public string? NumberOfTugsOut { get; set; }
        [Column("Tugs_Out_Start_Time")]
        public string? TugsOutStartTime { get; set; }
        [Column("Number_of_Shifting_Tugs")]
        public string? NumberOfShiftingTugs { get; set; }
        [Column("Shifting_Tugs_Start_Time")]
        public string? ShiftingTugsStartTime { get; set; }
        [Column("Berth_Shifted_To__In")]
        public string? BerthShiftedToIn { get; set; }
        [Column("Tugs_in_End_Time")]
        public string? TugsInEndTime { get; set; }
        [Column("Berth_Shifted_From")]
        public string? BerthShiftedFrom { get; set; }
        [Column("Tugs_Out_End_Time")]
        public string? TugsOutEndTime { get; set; }
        [Column("Berth_Shifted_To")]
        public string? BerthShiftedTo { get; set; }
        [Column("Shifting_Tugs_End_Time")]
        public string? ShiftingTugsEndTime { get; set; }
        [Column("__PEId")]
        public string? Peid { get; set; }
        public bool? IsNdrProcessed { get; set; }
        public bool? IsVrsProcessed { get; set; }
        public bool? IsUnifiedMetricsProcessed { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool? IsSentToVesLink { get; set; }
        [StringLength(20)]
        public string? ReportLocation { get; set; }  
        public decimal? ManouveringHours { get; set; }
        public decimal? MEPowerkW { get; set; }
        public decimal? DSS { get; set; }
        public decimal? HoursSinceLastReport { get; set; }
        public string? RemarksforOtherCons { get; set; }
        public string? StoppageRemarks { get; set; }
        public string? CargoDischarge { get; set; }
        public string? CargoLoad { get; set; }
        public string? GmtOffset { get; set; }    

        [InverseProperty("VesselRawData")]
        public virtual ICollection<TbVesselBunkerStemsAndSurveysRawDatum> TbVesselBunkerStemsAndSurveysRawData { get; set; }
        [InverseProperty("VesselDataRaw")]
        public virtual ICollection<TbVesselBunkerTankRawDatum> TbVesselBunkerTankRawData { get; set; }
        [InverseProperty("VesselDataRaw")]
        public virtual ICollection<TbVesselForobRawDatum> TbVesselForobRawData { get; set; }
        [InverseProperty("VesselDataRaw")]
        public virtual ICollection<TbVesselUpcomingPortRawDatum> TbVesselUpcomingPortRawData { get; set; }
        [InverseProperty("VesselDataRaw")]
        public virtual ICollection<TbVesselsAuxilliaryRawDatum> TbVesselsAuxilliaryRawData { get; set; }
        [InverseProperty("VesselDataRaw")]
        public virtual ICollection<TbCargoHandlingDatum> TbCargoHandlingData { get; set; }
        [InverseProperty("VesselDataRaw")]
        public virtual ICollection<TbPortActivities> TbPortActivitiesData { get; set; }
    }
}
