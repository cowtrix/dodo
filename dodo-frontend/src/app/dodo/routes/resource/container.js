import { connect } from "react-redux"
import { Resource } from "./resource"

import { resources, user } from "app/domain"
import { actions, selectors } from 'app/dodo/redux'
import { selectors as resourceSelectors } from 'app/domain/resources';

const { centerMap } = selectors
const { setCenterMap } = actions

const mapStateToProps = state => ({
	centerMap: centerMap(state),
	resource: resources.selectors.currentResource(state),
	notifications: resources.selectors.currentNotifications(state),
	error: resources.selectors.resourceError(state),
	hasFailed: resources.selectors.resourceFailed(state),
	isLoading: resources.selectors.resourceLoading(state),
	isLoadingNotifications: resources.selectors.notificationsLoading(state),
	resourceTypes: resources.selectors.resourceTypes(state),
	isLoggedIn: !!user.selectors.username(state),
	isMember: resourceSelectors.isMember(state),
	fetchingUser: user.selectors.fetchingUser(state)
})

const mapDispatchToProps = dispatch => ({
	setCenterMap: (centerMap) => dispatch(setCenterMap(centerMap)),
	getResource: (resourceType, slug, setCenterMap) => {
		resources.actions.resourceGet(dispatch, resourceType, slug, setCenterMap)
	},
	getNotifications: (resourceType, slug, page) => {
		resources.actions.notificationsGet(dispatch, resourceType, slug, page)
	},
	subscribeResource: (resourceType, slug, subscribe, body) =>
		resources.actions.subscribeResource(dispatch, resourceType, slug, subscribe, body),
})

export const ResourceConnected = connect(
	mapStateToProps,
	mapDispatchToProps
)(Resource)
