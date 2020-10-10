import { api } from './api-service';

function spyOnFetch(contentType = 'application/json', ok = true, responseText = '{"test": 123}') {
    const headers = new Map();
    headers.set('content-type', contentType);

    jest.spyOn(global, 'fetch')
    .mockImplementation(() => Promise.resolve({
        headers,
        ok,
        text: () => Promise.resolve(responseText)
    }));
}


describe('api-service tests', () => {
    beforeEach(() => {
        console.warn = jest.fn()
    })

    afterEach(() => {
        console.warn.mockRestore();
    })

    test('It should return parsed data upon receiving a successful response with valid JSON data and a content-type header of application/json', () => {
        spyOnFetch('application/json', true, '{"test": 123}');
        expect(api('http://test.com', 'get', {})).resolves.toEqual({
            test: 123
        })
    })

    test('It should reject upon receiving a successful response with invalid JSON data and a content-type header of application/json', () => {
        spyOnFetch('application/json', true, '{"test": 123}<>!');
        expect(api('http://test.com', 'get', {})).rejects.toBeDefined();
    })

    test('It should return parsed data upon receiving a successful response with valid JSON data and a content-type header other than application/json', async () => {
        spyOnFetch('text/plain', true, '{"test": 123}');
        const resp = await api('http://test.com', 'get', {});
        expect(resp).toEqual({ test: 123 });
        expect(console.warn).toHaveBeenCalled();
    })

    test('It should return text upon receiving a successful response with invalid JSON data and a content-type header other than application/json', async() => {
        spyOnFetch('text/plain', true, '{"test": 123}<>!');
        const resp = await api('http://test.com', 'get', {});
        expect(resp).toEqual('{"test": 123}<>!');
    })

    test('It should reject upon receiving an unsuccessful response', () => {
        spyOnFetch('text/plain', false, '{"test": 123}');
        expect(api('http://test.com', 'get', {})).rejects.toBeDefined();
    })
})