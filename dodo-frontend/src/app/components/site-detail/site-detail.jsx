import React from "react"
import { Link } from "react-router-dom"

import { PageTitle } from "app/components/page-title"
import { DatePage } from "app/components/date-page"
import { Button } from "app/components/button"

import styles from "./site-detail.module.scss"

export const SiteDetail = ({ site }) => {
	return (
		<DatePage
			startDate={site.StartDate ? new Date(site.StartDate) : null}
			endDate={site.EndDate ? new Date(site.EndDate) : null}
			title={<PageTitle title={site.Name} subTitle="Glasgow" />}
		>
			<div>{site.PublicDescription}</div>
			<div className={styles.attendees}>
				<Button
					to={"/site/join"}
					as={<Link />}
					type="button"
					style={{ display: "inline-block" }}
				>
					ATTEND EVENT
				</Button>
			</div>
		</DatePage>
	)
}
