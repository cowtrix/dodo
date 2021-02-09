import { useLocation } from "react-router";

export const useURLParams =
	/**
	 *@return {{[key: string]: string}}
	 */
	() => Object.fromEntries(new URLSearchParams(useLocation().search));
