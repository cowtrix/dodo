import { connect } from "react-redux"
import { Search } from "./search"
import { search } from "app/domain"

const mapStateToProps = state => ({
	latlong: search.selectors.latlong(state),
	distance: search.selectors.distance(state),
	initialSearchResults: search.selectors.searchResults(state),
	searchResults: search.selectors.searchResultsFiltered(state)
})

const mapDispatchToProps = dispatch => ({
	getSearchResults: (distance, latlong) =>
		search.actions.searchGet(dispatch, { distance, latlong })
})

export const SearchConnected = connect(
	mapStateToProps,
	mapDispatchToProps
)(Search)
