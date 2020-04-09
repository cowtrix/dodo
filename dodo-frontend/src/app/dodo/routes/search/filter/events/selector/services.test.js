import { formatEvents } from "./services";

const mockEvents = ["event1", "event2"];

const mockResult = [
	{
		value: "event1",
		label: "event1"
	},
	{
		value: "event2",
		label: "event2"
	}
];

describe("format events", () => {
	it("should take an array of events and format them for the select component", () => {
		expect(formatEvents(mockEvents)).toEqual(mockResult);
	});
});
