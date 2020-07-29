import { connect } from "react-redux"
import { Search } from "./search"
import { search, resources } from "app/domain"
import { actions as appActions, selectors as appSelectors } from 'app/dodo/redux'

const { centerMap } = appSelectors
const { setCenterMap } = appActions

const { selectors, actions } = search

const mapStateToProps = state => ({
	centerMap: centerMap(state),
	searchParams: selectors.searchParams(state),
	searchResults: selectors.searchResults(state),
	isFetchingSearch: selectors.isFetching(state),
	homeVideo: resources.selectors.homeVideo(state),
	resourceTypes: resources.selectors.resourceTypes(state)
})

const mapDispatchToProps = dispatch => ({
	setCenterMap: (centerMap) => dispatch(setCenterMap(centerMap)),
	getSearchResults: (searchParams, cb) =>
		actions.searchGet(dispatch, searchParams, cb),
	setSearchParams: (searchParams) => actions.setSearchParams(dispatch, searchParams)
})

export const SearchConnected = connect(
	mapStateToProps,
	mapDispatchToProps
)(Search)
