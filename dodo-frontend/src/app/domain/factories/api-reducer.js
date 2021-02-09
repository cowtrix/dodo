import { CANCELLED, FAILURE, REQUEST, SUCCESS } from "../constants";

const initialStateFactory = (actionType) => ({
	isFetching: false,
	hasErrored: false,
	totalFetching: 0,
});

export const apiReducerFactory = (actionType) => (
	state = initialStateFactory(actionType),
	action
) => {
	const totalFetching = Math.max(
		state.totalFetching + (action.type.includes(REQUEST) ? 1 : -1),
		0
	);
	switch (action.type) {
		case actionType + REQUEST:
		case actionType + SUCCESS:
		case actionType + CANCELLED: {
			return {
				...state,
				isFetching: !!totalFetching,
				totalFetching,
				hasErrored: false,
				error: undefined,
			};
		}
		case actionType + FAILURE: {
			return {
				...state,
				isFetching: false,
				totalFetching: 0,
				hasErrored: true,
				error: action.payload,
			};
		}
		default:
			return state;
	}
};

export const reducerFactory = (actionType, defaultState = []) => (
	state = defaultState,
	action
) => {
	switch (action.type) {
		case actionType + SUCCESS: {
			return action.payload;
		}
		default:
			return state;
	}
};
