import { formatEvents } from "./services"

const mockEvents = ["event1", "event2"]

const mockResult = [
	{
		value: "event1",
		label: "event1"
	},
	{
		value: "event2",
		label: "event2"
	}
]

describe("format resources", () => {
	it("should take an array of resources and format them for the select component", () => {
		expect(formatEvents(mockEvents)).toEqual(mockResult)
	})
})
