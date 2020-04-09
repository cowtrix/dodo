import { filterByEvent } from "./services"

const mockEvents = ["type1", "type3"]

const mockResults = [
	{
		id: 1,
		METADATA: {
			TYPE: "type1"
		}
	},
	{
		id: 2,
		METADATA: {
			TYPE: "type2"
		}
	},
	{
		id: 3,
		METADATA: {
			TYPE: "type3"
		}
	},
	{
		id: 4,
		METADATA: {
			TYPE: "type1"
		}
	},
	{
		id: 5,
		METADATA: {
			TYPE: "type4"
		}
	}
]

const mockResult = [
	{
		id: 1,
		METADATA: {
			TYPE: "type1"
		}
	},
	{
		id: 3,
		METADATA: {
			TYPE: "type3"
		}
	},
	{
		id: 4,
		METADATA: {
			TYPE: "type1"
		}
	}
]

describe("format events", () => {
	it("should take an array of results and filter them by event", () => {
		expect(filterByEvent(mockResults, mockEvents)).toEqual(mockResult)
	})
})
