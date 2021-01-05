import { useEffect } from "react";

export const useBeforeUnload =
	/**
	 * Adds a synchronous function to be run before the page unloads.
	 * Will re-add function if it changes, so use useRef or
	 * useCallback to prevent this occuring in every re-render.
	 *
	 * @param {(...args: any[]) => any} onUnload */
	(onUnload) => {
		useEffect(() => {
			window.addEventListener("beforeunload", onUnload);
			return () => window.removeEventListener("beforeunload", onUnload);
		}, [onUnload]);
	};
