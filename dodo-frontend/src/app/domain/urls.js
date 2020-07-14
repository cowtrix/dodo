const rootUrl = process.env.REACT_APP_ROOT_URL || "https://www.dodo.ovh/"
const apiUrl = process.env.REACT_APP_API_URL || rootUrl + "api/"
export const authUrl = process.env.REACT_APP_AUTH_URL || rootUrl + "auth/"

export const API_URL = apiUrl
export const REBELLIONS = apiUrl + "rebellions/"
export const LOCAL_GROUPS = apiUrl + "localgroups/"
export const SITES = apiUrl + "sites/"
export const SEARCH = apiUrl + "search"
export const LOGIN = authUrl + "login/" // note: not api root
export const LOGOUT_URL = authUrl + "logout"

export const RESET_PASSWORD = authUrl + "resetpassword"
export const REGISTER_USER = authUrl + "register"
export const RESEND_VALIDATION_EMAIL_URL = authUrl + "verifyemail"
export const AUTH_URL = authUrl