import { connect } from "react-redux"
import { Search } from "./search"
import { search } from "app/domain"

const { selectors, actions } = search

const mapStateToProps = state => ({
	latlong: selectors.latlong(state),
	distance: selectors.distance(state),
	initialSearchResults: selectors.searchResults(state),
	searchResults: selectors.searchResultsFiltered(state),
	isFetchingSearch: selectors.isFetching(state)
})

const mapDispatchToProps = dispatch => ({
	getSearchResults: (distance, latlong) =>
		actions.searchGet(dispatch, { distance, latlong }),
	searchSetCurrentLocation: () => actions.searchSetCurrentLocation(dispatch)
})

export const SearchConnected = connect(
	mapStateToProps,
	mapDispatchToProps
)(Search)
