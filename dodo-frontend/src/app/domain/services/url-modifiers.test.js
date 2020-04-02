import { addParamsToUrl } from './url-modifiers'

const mockUrl = "this.is.a.url"

const mockParams = {
	param1: "whocares98339",
	param2: "notme83?><",
	param3: "oneMoreforLuck"
}

const mockResult = "this.is.a.url?param1=whocares98339&param2=notme83?><&param3=oneMoreforLuck"

describe('add parameters to url', () => {
	it('should add all parameters to the url formatted with = and remove the last &', () => {
		expect(addParamsToUrl(mockUrl, mockParams)).toBe(mockResult)
	})
})
