import React from "react"
import { ContentPage } from "app/components"
import { PageTitle } from "app/components"
import styles from "./error-boundary.module.scss"
import PropTypes from "prop-types"

export class ErrorBoundary extends React.Component {
	state = { hasError: false }

	static getDerivedStateFromError = error => ({ hasError: true })

	render() {
		if (this.state.hasError) {
			return this.props.errorContent ? (
				this.props.errorContent
			) : (
				<ContentPage>
					<div className={styles.errorBoundaryContainer}>
						<PageTitle
							title="Error"
							subTitle="Oops, something went wrong. Please refresh and try again."
						/>
					</div>
				</ContentPage>
			)
		}

		return this.props.children
	}
}

ErrorBoundary.propTypes = {
	errorContent: PropTypes.element
}
