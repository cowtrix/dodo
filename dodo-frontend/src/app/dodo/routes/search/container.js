import { connect } from "react-redux"
import { Search } from "./search"
import { search } from "app/domain"

const { selectors, actions } = search

const mapStateToProps = state => ({
	search: selectors.search(state),
	latlong: selectors.latlong(state),
	distance: selectors.distance(state),
	initialSearchResults: selectors.searchResults(state),
	searchResults: selectors.searchResultsFiltered(state),
	isFetchingSearch: selectors.isFetching(state)
})

const mapDispatchToProps = dispatch => ({
	getSearchResults: (distance, latlong, search) =>
		actions.searchGet(dispatch, { distance, latlong, search })
})

export const SearchConnected = connect(
	mapStateToProps,
	mapDispatchToProps
)(Search)
