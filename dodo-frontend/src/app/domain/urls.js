const rootUrl = process.env.REACT_APP_ROOT_URL || "https://www.dodo.ovh/"
const apiUrl = process.env.REACT_APP_API_URL || rootUrl + "api/"
const authUrl = process.env.REACT_APP_AUTH_URL || rootUrl + "auth/"

export const API_URL = apiUrl
export const REBELLIONS = apiUrl + "rebellions/"
export const LOCAL_GROUPS = apiUrl + "localgroups/"
export const SITES = apiUrl + "sites/"
export const SEARCH = apiUrl + "search"
export const LOGIN = authUrl + "login/" // note: not api root

export const RESET_PASSWORD = authUrl + "resetpassword"
export const REGISTER_USER = authUrl + "register"