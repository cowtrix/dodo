import React from "react"
import { Link } from "react-router-dom"

import { PageTitle } from "app/components/page-title"
import { DateLayout } from "app/components/date-layout"
import { Button } from "app/components/button"

import styles from "./site-detail.module.scss"

export const SiteDetail = ({ site }) => {
	return (
		<DateLayout
			startDate={site.StartDate ? new Date(site.StartDate) : null}
			endDate={site.EndDate ? new Date(site.EndDate) : null}
			title={<PageTitle title={site.Name} subTitle="Glasgow" />}
		>
			<div>{site.PublicDescription}</div>
			<div className={styles.attendees}>
				<Button
					as={<Link to={"/site/join"} />}
					type="button"
					style={{ display: "inline-block" }}
				>
					ATTEND EVENT
				</Button>
			</div>
		</DateLayout>
	)
}
