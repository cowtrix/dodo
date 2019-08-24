using CSVTools;

namespace XR.OctoberANFormAnalyzer
{
	class XRUserResponse : SchemaObject
	{
		public string Name;
		[Alias("Phone Number")]
		public string PhoneNumber;
		public string Email;
		public byte Age;
		[Alias("How long have you been involved in the Rebellion?")]
		public EInvolvementLength InvolvementLength;
		[Alias("How long have you been involved in the Rebellion?")]
		public enum EInvolvementLength
		{
			[Alias("I’m just joining the Rebellion.")]
			JustJoining,
			[Alias("Less than 3 months")]
			LessThan3Month,
			[Alias("3+ Months")]
			MoreThan3Months,
			[Alias("6+ Months")]
			MoreThan6Months,
			[Alias("9+months")]
			MoreThan9Months,
			[Alias("1+ year")]
			MoreThan1Year,
		}
		[Alias("Which part of the UK are you coming from?")]
		public ERegion Region;
		[Alias("Which part of the UK are you coming from?")]
		public enum ERegion
		{
			[Alias("South East")]
			SouthEast,
			[Alias("South West")]
			SouthWest,
			[Alias("East of England")]
			EastOfEngland,
			[Alias("East + West Midlands")]
			EastWestMidlands,
			[Alias("Wales")]
			Wales,
			[Alias("North West England, Yorkshire and the Humber, North East England")]
			NorthWest,
			[Alias("Scotland")]
			Scotland,
			[Alias("Northern Ireland")]
			NorthernIreland,
			[Alias("Greater London")]
			GreaterLondon,
		}
		[Alias("What local group are you in or nearest?")]
		public string LocalGroup;
		[Alias("Will you be joining us on October 7th!")]
		public bool ComingToOct7;		
		[Alias("How much time do you pledge to the Rebellion in October?")]
		public ETimeCommitmentInOctober TimeCommitmentInOctober;
		public enum ETimeCommitmentInOctober
		{
			[Alias("1 day")]
			OneDay,
			[Alias("2-3 days")]
			TwoToThreeDays,
			[Alias("1 week")]
			OneWeek,
			[Alias("2 weeks")]
			TwoWeeks,
			[Alias("3 weeks")]
			ThreeWeeks,
			[Alias("As long as it takes.")]
			AsLongAsItTakes,
		}
		[Alias("Have you been trained in Non-Violent Direct Action?")]
		public bool NVDATraining;
		[Alias("Are you willing to risk arrest?")]
		public bool WillRiskArrest;
		[Alias("Are you willing to go to prison?")]
		public bool WillGoToPrison;
		[Alias("Would you consider going on hunger strike?")]
		public bool WillGoOnHungerStrike;
		[Alias("Are you in an affinity Group?")]
		public bool IsInAffinityGroup;
		[Alias("Are you an affinity group coordinator?")]
		public bool IsAffinityGroupCoordinator;
		[Alias("What is the name of your affinity group?")]
		public string AffinityGroupName;
		[Alias("Would you like to be part of an Affinity Group?")]
		public bool WouldLikeToBePartOfAffinityGroup;		
		[Alias("Working Group")]
		public EWorkingGroup WorkingGroup;
		[Alias("Working Group")]
		public enum EWorkingGroup
		{
			[Alias("Action Support")]
			ActionSupport,
			[Alias("World Building and Production")]
			WorldBuildingAndProduction,
			[Alias("Media and Messaging")]
			MediaAndMessaging,
			[Alias("Movement Support")]
			MovementSupport,
		}
		[Alias("How much time do you have to give to your role prior to the rebellion?")]
		public enum ETimeCommitmentBeforeOctober
		{
			[Alias("All of my time")]
			AllOfMyTime,
			[Alias("5 days a week")]
			FiveDaysAWeek,
			[Alias("3 days a week")]
			ThreeDaysAWeek,
			[Alias("One day a week")]
			OneDayAWeek,
			[Alias("A few hours in the evenings")]
			FewHoursInEvening,
		}
		[Alias("How much time do you have to give to your role prior to the rebellion?")]
		public ETimeCommitmentBeforeOctober TimeCommitmentBeforeOctober;

		public XRUserResponse(CharSeperatedSpreadsheetReader.Row row) : base(row)
		{
		}

		public override string ToString()
		{
			return $"{Name}";
		}
	}

	
}
